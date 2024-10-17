using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            var list = await _unitOfWork.UserRepo.GetQueryable(query => query
                                                                     .Include(o => o.Role!)
                                                                     .Include(o => o.Picture!)
                                                                     .Include(o => o.Partners));
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

            // hand mapping for

            result.RoleName = _unitOfWork.RoleRepo.SingleOrDefaultAsync((u => u.Id == user.RoleId)).Result.RoleName;

            return result;
        }

        public async Task<UserListDTO> Login(string email, string password)
        {
            var exist = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Email == email,
                                                                        query => query.Include(o => o.Role));
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

            string generatePassword = GeneratePassword(Constants.Smtp.DEFAULT_PASSWORD_LENGTH);

            result.Password = BCrypt.Net.BCrypt.HashPassword(generatePassword);

            await _unitOfWork.UserRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();

            PasswordMailSender(result.Email, generatePassword);

            return ResponseMessage.Success;
        }

        public async Task<string> UpdateUser(UserUpdateRequestDTO user)
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


                _unitOfWork.UserRepo.Update(exist);
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
