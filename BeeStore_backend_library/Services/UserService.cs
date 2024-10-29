﻿using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;

namespace BeeStore_Repository.Services
{
    public class UserService : IUserService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public UserService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CreateEmployee(EmployeeCreateRequest user)
        {
            var exist = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (exist != null)
            {
                throw new DuplicateException(ResponseMessage.UserEmailDuplicate);
            }
            var result = _mapper.Map<Employee>(user);
            await _unitOfWork.EmployeeRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> DeleteEmployee(int id)
        {
            var exist = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist != null)
            {
                _unitOfWork.EmployeeRepo.SoftDelete(exist);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            return ResponseMessage.Success;
        }

        public async Task<Pagination<EmployeeListDTO>> GetAllEmployees(string search, EmployeeRole? role, UserSortBy? sortCriteria,
                                                              bool order, int pageIndex, int pageSize)
        {

            string? filterQuery = role switch
            {
                EmployeeRole.Admin => Constants.RoleName.Admin,
                EmployeeRole.Manager => Constants.RoleName.Manager,
                EmployeeRole.Staff => Constants.RoleName.Staff,
                EmployeeRole.Shipper => Constants.RoleName.Shipper,
                _ => null
            };

            string? sortBy = sortCriteria switch
            {
                UserSortBy.Email => Constants.SortCriteria.Email,
                UserSortBy.CreateDate => Constants.SortCriteria.CreateDate,
                UserSortBy.FirstName => Constants.SortCriteria.FirstName,
                UserSortBy.LastName => Constants.SortCriteria.LastName,
                _ => null
            };

            var list = await _unitOfWork.EmployeeRepo.GetListAsync(
                filter: u => filterQuery == null || u.Role!.RoleName!.Equals(filterQuery),
                includes: query => query.Include(o => o.Role),
                sortBy: sortBy!,
                descending: order,
                searchTerm: search,
                searchProperties: new Expression<Func<Employee, string>>[] { p => p.Email }
                );


            var result = _mapper.Map<List<EmployeeListDTO>>(list);


            return await ListPagination<EmployeeListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<EmployeeListDTO> GetEmployee(string email)
        {
            var user = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Email.Equals(email),
                                                                       query => query.Include(o => o.Role));
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
            }
            var result = _mapper.Map<EmployeeListDTO>(user);

            return result;
        }

        public async Task<UserLoginResponseDTO> Login(string email, string password)
        {
            var exist = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Email == email,
                                                                        query => query.Include(o => o.Role));
            
            var partner = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Email == email,
                                                                                 query => query.Include(o => o.Role));

            if (partner == null && exist == null)
            {

                throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
            }

            if (exist != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, exist.Password))
                {
                    return new UserLoginResponseDTO(exist.Email, exist.Role!.RoleName!);
                }
                else
                {
                    throw new KeyNotFoundException(ResponseMessage.UserPasswordError);
                }
            }


            if (partner != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, partner.Password))
                {
                    return new UserLoginResponseDTO(partner.Email, partner.Role!.RoleName!);
                }
                else
                {
                    throw new KeyNotFoundException(ResponseMessage.UserPasswordError);
                }
            }
            return null;
        }

        public async Task<string> SignUp(UserSignUpRequestDTO request)
        {
            var exist = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (exist != null)
            {
                throw new DuplicateException(ResponseMessage.UserEmailDuplicate);
            }
            
            var result = _mapper.Map<OcopPartner>(request);
            result.RoleId = 2;
            string generatePassword = GeneratePassword(Constants.Smtp.DEFAULT_PASSWORD_LENGTH);

            result.Password = BCrypt.Net.BCrypt.HashPassword(generatePassword);

            await _unitOfWork.OcopPartnerRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();

            PasswordMailSender(result.Email, generatePassword);

            return ResponseMessage.Success;
        }

        public async Task<string> UpdateEmployee(EmployeeUpdateRequest user)
        {
            var exist = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (exist != null)
            {
                //CHECK OLD PASSWORD
                if (!BCrypt.Net.BCrypt.Verify(user.ConfirmPassword, exist.Password))
                {
                    throw new ApplicationException(ResponseMessage.UserPasswordError);
                }


                exist.Setting = user.Setting;
                exist.PictureLink = user.PictureLink;
                if (!String.IsNullOrEmpty(user.Phone) && !user.Phone.Equals(Constants.DefaultString.String))
                {
                    exist.Phone = user.Phone;
                }
                if (!String.IsNullOrEmpty(user.FirstName) && !user.FirstName.Equals(Constants.DefaultString.String))
                {
                    exist.FirstName = user.FirstName;
                }
                if (!String.IsNullOrEmpty(user.LastName) && !user.LastName.Equals(Constants.DefaultString.String))
                {
                    exist.LastName = user.LastName;
                }


                _unitOfWork.EmployeeRepo.Update(exist);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
            }

            return ResponseMessage.Success;
        }

        


        private void PasswordMailSender(string targetMail, string userGeneratedPassword)
        {
            if (userGeneratedPassword is null)
            {
                throw new ArgumentNullException(nameof(userGeneratedPassword));
            }
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(Constants.DefaultString.systemJsonFile).Build();

            var mailConfig = config.GetSection("Mail").Get<AppConfiguration>();

            var keyVault = config.GetSection("KeyVault").Get<AppConfiguration>();

            var _client = new SecretClient(new Uri(keyVault.KeyVaultURL), new EnvironmentCredential());

            string smtpPassword = _client.GetSecret("BeeStore-Smtp-Password").Value.Value;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(mailConfig.sourceMail);
            mailMessage.Subject = Constants.Smtp.registerMailSubject;
            mailMessage.To.Add(new MailAddress(targetMail));
            // Ignore this abomination below
            mailMessage.Body = @"
                                <html>
                                  <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; border-radius: 10px;'>
                                      <h2 style='color: #4CAF50;'>Welcome to BeeShelf!</h2>
                                      <p>Dear User,</p>
                                      <p>Your account has been verified. Here is the password for your account:</p>
                                      <p style='font-weight: bold; font-size: 18px; color: #333;'>Your Password: 
                                        <span style='color: #d9534f;'>" + userGeneratedPassword + @"</span>
                                      </p>
                                      <p>For security reasons, we recommend that you change your password as soon as possible.</p>
                                      <p>If you didn't request this password, please contact our support team immediately.</p>
                                      <p>Thank you for using our service!</p>
                                      <p style='margin-top: 30px; font-size: 12px; color: #888;'>This is an automated email, please do not reply.</p>
                                    </div>
                                  </body>
                                </html>";
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient(Constants.Smtp.smtp)
            {
                Port = 587,
                Credentials = new NetworkCredential(mailConfig.sourceMail, smtpPassword),
                EnableSsl = true
            };

            smtpClient.Send(mailMessage);
        }

        private static string GeneratePassword(int length)
        {

            char[] password = new char[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[1];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(randomBytes);
                    int randomIndex = randomBytes[0] % Constants.Smtp.characterSet.Length;
                    password[i] = Constants.Smtp.characterSet[randomIndex];
                }
            }

            return new string(password);
        }

    }
}
