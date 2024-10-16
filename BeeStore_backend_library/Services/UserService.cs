using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Castle.Core.Configuration;
using Microsoft.EntityFrameworkCore;
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
        private readonly SecretClient _client = new SecretClient(new Uri("https://beestore-keyvault.vault.azure.net/"), new EnvironmentCredential());;

        private const int DEFAULT_PASSWORD_LENGTH = 12;
        private const string lowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string digits = "0123456789";
        private const string specialCharacters = "!@#$%^&*()_+-=[]{}|;:',.<>?/";
        private const string characterSet = lowercaseLetters + uppercaseLetters + digits + specialCharacters;
        public UserService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CreateUser(UserCreateRequestDTO user)
        {
            var exist = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (exist != null)
            {
                throw new DuplicateException(ResponseMessage.UserEmailDuplicate);
            }
            var result = _mapper.Map<User>(user);
            await _unitOfWork.UserRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> DeleteUser(int id)
        {
            var exist = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist != null)
            {
                _unitOfWork.UserRepo.SoftDelete(exist);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            return ResponseMessage.Success;
        }

        public async Task<Pagination<UserListDTO>> GetAllUser(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.UserRepo.GetAllAsync();
            if (list.Count == 0)
            {
                throw new ApplicationException();
            }

            var result = _mapper.Map<List<UserListDTO>>(list);


            return await ListPagination<UserListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<UserListDTO> GetUser(string email)
        {
            var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Email.Equals(email));
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var result = _mapper.Map<UserListDTO>(user);
            return result;
        }

        public async Task<UserListDTO> Login(string email, string password)
        {
            var exist = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Email == email);
            if (exist != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, exist.Password))
                {
                    return _mapper.Map<UserListDTO>(exist);
                }
                else
                {
                    throw new KeyNotFoundException(ResponseMessage.UserPasswordError);
                }
            }
            else
            {
                throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
            }
        }

        public async Task<string> SignUp(UserSignUpRequestDTO request)
        {
            var exist = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (exist != null)
            {
                throw new DuplicateException(ResponseMessage.UserEmailDuplicate);
            }
            if (request.RoleName != Constants.RoleName.User)
            {
                throw new ApplicationException();
            }
            var result = _mapper.Map<User>(request);

            string generatePassword = GeneratePassword(DEFAULT_PASSWORD_LENGTH);

            result.Password = BCrypt.Net.BCrypt.HashPassword(generatePassword);

            await _unitOfWork.UserRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();

            PasswordMailSender(result.Email, generatePassword);

            return ResponseMessage.Success;
        }

        public async Task<UserUpdateRequestDTO> UpdateUser(UserUpdateRequestDTO user)
        {
            var exist = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (exist != null)
            {
                //CHECK OLD PASSWORD
                if (BCrypt.Net.BCrypt.Verify(user.ConfirmPassword, exist.Password))
                {
                    throw new ApplicationException(ResponseMessage.UserPasswordError);
                }
                if (!user.PictureId.Equals(0))
                {
                    exist.PictureId = user.PictureId;
                }
                if (!String.IsNullOrEmpty(user.Phone) && !user.Phone.Equals("string"))
                {
                    exist.Phone = user.Phone;
                }
                if (!String.IsNullOrEmpty(user.FirstName) && !user.FirstName.Equals("string"))
                {
                    exist.FirstName = user.FirstName;
                }
                if (!String.IsNullOrEmpty(user.LastName) && !user.LastName.Equals("string"))
                {
                    exist.LastName = user.LastName;
                }


                _unitOfWork.UserRepo.Update(exist);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
            }

            return user;
        }


        private void PasswordMailSender(string targetMail, string userGeneratedPassword)
        {
            if (userGeneratedPassword is null)
            {
                throw new ArgumentNullException(nameof(userGeneratedPassword));
            }

            string sourceMail = "beeshelf.notification@gmail.com";
            string smtpPassword = _client.GetSecret("BeeStore-Smtp-Password").Value.Value;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(sourceMail);
            mailMessage.Subject = "BeeShelf Account Password";
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

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(sourceMail, smtpPassword),
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
                    int randomIndex = randomBytes[0] % characterSet.Length;
                    password[i] = characterSet[randomIndex];
                }
            }

            return new string(password);
        }

    }
}
