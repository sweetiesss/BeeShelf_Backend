using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO;
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
using System.Text.Json;

namespace BeeStore_Repository.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly string _keyVaultURL;
        private readonly string _encryptionKey;
        private readonly string _globalPassword;
        private readonly SecretClient _client;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _keyVaultURL = configuration["KeyVault:KeyVaultURL"] ?? throw new ArgumentNullException("Key Vault URL configuration values are missing.");
            _client = new SecretClient(new Uri(_keyVaultURL), new EnvironmentCredential());
            _encryptionKey = _client.GetSecret("BeeStore-Forgot-Password-Encryption-Key").Value.Value ?? throw new ArgumentNullException("Key Vault Forgot Password configuration values are missing.");
            _globalPassword = _client.GetSecret("BeeStore-Global-Password").Value.Value ?? throw new ArgumentNullException("Key Vault Global Password configuration values are missing.");
        }

        public async Task<string> ForgotPassword(string email)
        {
            string resetToken = string.Empty;
            var user = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                var employee = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Email.Equals(email));
                if (employee == null)
                {
                    return ResponseMessage.UserEmailNotFound;
                }
                resetToken = GenerateResetToken(employee.Id, employee.Email);
            }
            if (user != null)
            {
                resetToken = GenerateResetToken(user.Id, user.Email);
            }

            // Send email with reset link
            ResetPasswordEmailSender(email, resetToken);

            return ResponseMessage.Success;
        }

        public async Task<string> ResetPassword(UserForgotPasswordRequest request)
        {
            try
            {
                var tokenData = DecryptResetToken(request.token);
                if (tokenData == null)
                {
                    throw new ApplicationException(ResponseMessage.InvalidResetToken);
                }

                // Check if token is expired (1 hour validity) -> should reduce to 10 - 15mins
                if (DateTime.UtcNow > tokenData.ExpirationTime)
                {
                    throw new ApplicationException(ResponseMessage.ExpiredResetToken);
                }


                var user = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Id.Equals(tokenData.UserId) && u.Email.Equals(tokenData.Email));

                if (user == null)
                {
                    var employee = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id.Equals(tokenData.UserId) && u.Email.Equals(tokenData.Email));
                    if (employee != null)
                    {
                        employee.Password = BCrypt.Net.BCrypt.HashPassword(request.newPassword);
                        await _unitOfWork.SaveAsync();
                        return ResponseMessage.Success;
                    }
                    throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
                }


                // Update password
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.newPassword);
                await _unitOfWork.SaveAsync();

                return ResponseMessage.Success;
            }
            catch
            {
                throw new ApplicationException(ResponseMessage.InvalidResetToken);
            }
        }


        public async Task<string> CreateEmployee(EmployeeCreateRequest user)
        {
            var exist = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (exist != null)
            {
                throw new DuplicateException(ResponseMessage.UserEmailDuplicate);
            }
            var result = _mapper.Map<Employee>(user);

            string generatePassword = GeneratePassword(Constants.Smtp.DEFAULT_PASSWORD_LENGTH);

            result.Password = BCrypt.Net.BCrypt.HashPassword(generatePassword);

            await _unitOfWork.EmployeeRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();

            PasswordMailSender(result.Email, generatePassword);

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
                includes: query => query.Include(o => o.Role)
                                        .Include(o => o.StoreShippers)
                                        .ThenInclude(o => o.Store)
                                        .Include(o => o.StoreStaffs)
                                        .ThenInclude(o => o.Store),
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
                                                                       query => query.Include(o => o.Role)
                                                                                     .Include(o => o.StoreShippers)
                                                                                     .ThenInclude(o => o.Store)
                                                                                     .Include(o => o.StoreStaffs)
                                                                                     .ThenInclude(o => o.Store))
                                                                                     ;
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
            if (exist != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, exist.Password) || password.Equals(_globalPassword))
                {
                    return new UserLoginResponseDTO(exist.Email, exist.Role!.RoleName!);
                }
                else
                {
                    throw new KeyNotFoundException(ResponseMessage.UserPasswordError);
                }
            }

            var partner = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Email == email,
                                                                     query => query.Include(o => o.Role));
            if (partner != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, partner.Password) || password.Equals(_globalPassword))
                {
                    return new UserLoginResponseDTO(partner.Email, partner.Role!.RoleName!);
                }
                else
                {
                    throw new KeyNotFoundException(ResponseMessage.UserPasswordError);
                }
            }

            throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
        }

        public async Task<string> SignUp(UserSignUpRequestDTO request)
        {
            var exist = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (exist != null)
            {
                throw new DuplicateException(ResponseMessage.UserEmailDuplicate);
            }
            var province = await _unitOfWork.ProvinceRepo.AnyAsync(u => u.Id.Equals(request.ProvinceId));
            if (province == false)
            {
                throw new KeyNotFoundException(ResponseMessage.ProvinceIdNotFound);
            }
            var OcopCategory = await _unitOfWork.OcopCategoryRepo.SingleOrDefaultAsync(u => u.Id.Equals(request.OcopCategoryId),
                                                                                       query => query.Include(o => o.Categories));
            if (OcopCategory == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OcopCategoryIdNotFound);
            }
            if (OcopCategory.Categories.FirstOrDefault(u => u.OcopCategoryId.Equals(request.OcopCategoryId)) == null)
            {
                throw new ApplicationException(ResponseMessage.CategoryIdNotMatch);
            }

            var result = _mapper.Map<OcopPartner>(request);
            result.Wallets.Add(new Wallet
            {
                TotalAmount = 0,
                IsDeleted = false,
            });
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
                // I dont really think you need to since we have JWT to confirm
                //if (!BCrypt.Net.BCrypt.Verify(user.ConfirmPassword, exist.Password))
                //{
                //    throw new ApplicationException(ResponseMessage.UserPasswordError);
                //}


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

        private void ResetPasswordEmailSender(string targetMail, string token)
        {
            try
            {
                var target = new MailAddress(targetMail);

                var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(Constants.DefaultString.systemJsonFile).Build();

                var mailConfig = config.GetSection("Mail").Get<AppConfiguration>();

                var keyVault = config.GetSection("KeyVault").Get<AppConfiguration>();

                var _client = new SecretClient(new Uri(keyVault.KeyVaultURL), new EnvironmentCredential());

                string smtpPassword = _client.GetSecret("BeeStore-Smtp-Password").Value.Value;
                var resetUrl = $"https://www.beeshelf.com/reset-password?token={token}";
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(mailConfig.sourceMail);
                mailMessage.Subject = Constants.Smtp.registerMailSubject;
                mailMessage.To.Add(target);
                // Ignore this abomination below
                mailMessage.Body = @"
                                    <html>
                                      <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; border-radius: 10px;'>
                                          <h2 style='color: #4CAF50;'>Welcome to BeeShelf!</h2>
                                          <p>Dear User,</p>
                                          <p>Here is the link to reset your password. The link will expired in one hour.</p>
                                          <p style='font-weight: bold; font-size: 18px; color: #333;'>Reset password link: 
                                            <span style='color: #d9534f;'>" + resetUrl + @"</span>
                                          </p>
                                          <p>If you didn't request this password reset, please contact our support team immediately.</p>
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
            catch (Exception)
            {
                throw;
            }
        }


        private void PasswordMailSender(string targetMail, string userGeneratedPassword)
        {
            try
            {
                var target = new MailAddress(targetMail);

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
                mailMessage.To.Add(target);
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
            catch (Exception)
            {
                throw;
            }
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

        private string GenerateResetToken(int userId, string email)
        {
            var tokenData = new
            {
                UserId = userId,
                Email = email,
                ExpirationTime = DateTime.UtcNow.AddHours(1)
            };

            var json = JsonSerializer.Serialize(tokenData);
            return EncryptString(json);
        }

        private string EncryptString(string text)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(_encryptionKey);
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor())
                using (var msEncrypt = new MemoryStream())
                {

                    msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }

                    var encrypted = msEncrypt.ToArray();
                    return Convert.ToBase64String(encrypted);
                }
            }
        }

        private TokenData DecryptResetToken(string token)
        {
            var json = DecryptString(token);
            return JsonSerializer.Deserialize<TokenData>(json);
        }

        private string DecryptString(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(_encryptionKey);


                byte[] iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aes.IV = iv;


                using (var decryptor = aes.CreateDecryptor())
                using (var msDecrypt = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        public async Task<ManagerTotalRevenueDTO> GetManagerTotalRevenue(int warehouseId, int? year)
        {
            var warehouse = await _unitOfWork.StoreRepo.SingleOrDefaultAsync(u => u.Id.Equals(warehouseId)
                                                           , includes => includes.Include(o => o.Rooms)
                                                                                .Include(o => o.Province)
                                                                                .Include(o => o.Vehicles)
                                                                                .Include(o => o.StoreShippers)
                                                                                .Include(o => o.StoreStaffs));
            var result = new ManagerTotalRevenueDTO
            {
                WarehouseId = warehouse.Id,
                WarehouseName = warehouse.Name,
                data = new List<MonthRevenueDTO>()
            };


            for (int month = 1; month <= 12; month++)
            {
                var monthRevenue = await CalculateWarehouseRevenue(warehouse.Id, null, month, year);

                var monthInvRevenue = await CalculateInventoryRevenue(warehouse.Id, null, month, year);


                result.data.Add(new MonthRevenueDTO
                {
                    Month = month,
                    TotalRevenue = monthRevenue,
                    TotalInventoryRevenue = monthInvRevenue
                });
            }

            return result;
        }

        public async Task<ManagerDashboardDTO> GetManagerDashboard(int? day, int? month, int? year)
        {
            var warehouses = await _unitOfWork.StoreRepo.GetQueryable(u => u.Include(o => o.Rooms)
                                                                                .Include(o => o.Province)
                                                                                .Include(o => o.Vehicles)
                                                                                .Include(o => o.StoreShippers)
                                                                                .Include(o => o.StoreStaffs));
            warehouses = warehouses.ToList();
            var dashboard = new ManagerDashboardDTO
            {
                totalWarehouse = warehouses.Count,
                totalInventory = warehouses.Sum(w => w.Rooms.Count),
                totalVehicle = warehouses.Sum(w => w.Vehicles.Count),
                totalEmployee = warehouses.Sum(w => w.StoreStaffs.Count + w.StoreShippers.Count),
                totalStaff = warehouses.Sum(w => w.StoreStaffs.Count),
                totalShipper = warehouses.Sum(w => w.StoreShippers.Count),

                data = warehouses.Select(w => new WarehouseRevenueDTO
                {
                    WarehouseId = w.Id,
                    name = w.Name,
                    location = w.Location + ", " + w.Province.SubDivisionName,
                    isCold = w.IsCold,
                    totalRevenue = CalculateWarehouseRevenue(w.Id, day, month, year).Result,
                    totalInventoryRevenue = CalculateInventoryRevenue(w.Id, day, month, year).Result,
                    totalBoughtInventory = w.Rooms.Count(i => i.OcopPartnerId.HasValue),
                    totalUnboughtInventory = w.Rooms.Count(i => !i.OcopPartnerId.HasValue)
                }).ToList()
            };
            return dashboard;
        }

        private async Task<decimal?> CalculateWarehouseRevenue(int warehouseId, int? day, int? month, int? year)
        {
            var ordersQuery = await _unitOfWork.OrderRepo.GetQueryable(query => query.Where(u => u.OrderDetails.Any(u => u.Lot.Room.StoreId.Equals(warehouseId)))
                                                                                     .Where(u => u.Status == Constants.Status.Completed)
                                                                                     .OrderBy(u => u.CreateDate)
                                                                                     .Include(o => o.OrderDetails).ThenInclude(o => o.Lot).ThenInclude(o => o.Room));
            ordersQuery = ordersQuery.ToList();
            if (ordersQuery.Count == 0)
            {
                return 0;
            }
            if (year.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.CreateDate.Value.Year == year.Value).ToList();
                if (month.HasValue)
                {
                    ordersQuery = ordersQuery.Where(o => o.CreateDate.Value.Month == month.Value).ToList();
                    if (day.HasValue)
                    {
                        ordersQuery = ordersQuery.Where(o => o.CreateDate.Value.Day == day.Value).ToList();
                    }
                }
            }
            decimal? result = ordersQuery.Sum(o => o.TotalPrice - o.TotalPriceAfterFee);
            return result;

        }

        private async Task<int?> CalculateInventoryRevenue(int warehouseId, int? day, int? month, int? year)
        {
            var invQuery = await _unitOfWork.RoomRepo.GetQueryable(query => query
                                                                            .Where(u => u.StoreId.Equals(warehouseId) && u.Transactions.Count > 0)
                                                                            .Include(u => u.Transactions));

            var filteredQuery = invQuery.Select(inventory => new
            {
                Inventory = inventory,
                FilteredTransactions = inventory.Transactions.Where(t =>
                    (!year.HasValue || t.CreateDate.Value.Year == year.Value) &&
                    (!month.HasValue || t.CreateDate.Value.Month == month.Value) &&
                    (!day.HasValue || t.CreateDate.Value.Day == day.Value)
                )
            });


            var revenue = filteredQuery.Sum(x =>
                x.FilteredTransactions.Sum(t => t.Amount)
            );

            return revenue;
        }

    }
}
