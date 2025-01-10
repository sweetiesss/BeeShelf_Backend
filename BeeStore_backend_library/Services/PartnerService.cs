using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.DTO.ProvinceDTOs;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Net;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.ProductDTOs;

namespace BeeStore_Repository.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPictureService _pictureService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public PartnerService(IUnitOfWork unitOfWork,IPictureService pictureService, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _pictureService = pictureService;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<Pagination<PartnerListDTO>> GetAllPartners(string search, SortBy? sortby, bool descending, int pageIndex, int pageSize)
        {
            string sortCriteria = sortby.ToString()!;

            var list = await _unitOfWork.OcopPartnerRepo.GetListAsync(
                filter: null!,
                includes: o => o.Include(o => o.Province)
                                .Include(o => o.Category)
                                .Include(o => o.OcopCategory)
                                .Include(o => o.Role),
                sortBy: sortCriteria!,
                descending: descending,
                searchTerm: search!,
                searchProperties: new Expression<Func<OcopPartner, string>>[] { p => p.Email, p => p.FirstName,
                                                                                p => p.LastName, p => p.BusinessName}
                );

            var result = _mapper.Map<List<PartnerListDTO>>(list);

            return await ListPagination<PartnerListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<PartnerListDTO> GetPartner(string email)
        {
            var partner = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Email == email,
                                                                               query => query.Include(o => o.Province)
                                                                                             .Include(o => o.Category)
                                                                                             .Include(o => o.OcopCategory)
                                                                                             .Include(o => o.Role));
            if (partner == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
            }
            var result = _mapper.Map<PartnerListDTO>(partner);
            return result;
        }



        public async Task<string> UpdatePartner(OCOPPartnerUpdateRequest user)
        {
            var exist = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (exist != null)
            {
                // Same thing here
                //if (!BCrypt.Net.BCrypt.Verify(user.ConfirmPassword, exist.Password))
                //{
                //    throw new ApplicationException(ResponseMessage.UserPasswordError);
                //}
                if ((DateTime.Now - exist.UpdateDate.Value).TotalDays < 30)
                {
                    throw new ApplicationException(ResponseMessage.UpdatePartnerError);
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
                var province = await _unitOfWork.OcopPartnerRepo.AnyAsync(u => u.Id.Equals(user.ProvinceId));
                if (province == false)
                {
                    throw new KeyNotFoundException(ResponseMessage.ProvinceIdNotFound);
                }
                var OcopCategory = await _unitOfWork.OcopCategoryRepo.SingleOrDefaultAsync(u => u.Id.Equals(user.OcopCategoryId));
                if (OcopCategory == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.OcopCategoryIdNotFound);
                }
                if (OcopCategory.Categories.Any(u => u.Id == user.OcopCategoryId) == false)
                {
                    throw new ApplicationException(ResponseMessage.CategoryIdNotMatch);
                }
                exist.BankAccountNumber = user.BankAccountNumber;
                exist.BankName = user.BankName;
                exist.BusinessName = user.BusinessName;
                exist.CategoryId = user.CategoryId;
                exist.ProvinceId = user.ProvinceId;
                exist.TaxIdentificationNumber = user.TaxIdentificationNumber;
                exist.OcopCategoryId = user.OcopCategoryId;
                exist.UpdateDate = DateTime.Now;
                _unitOfWork.OcopPartnerRepo.Update(exist);
                await _unitOfWork.SaveAsync();
                return ResponseMessage.Success;
            }
            else
            {
                throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
            }

        }

        public async Task<string> DeletePartner(int id)
        {
            var exist = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PartnerIdNotFound);
            }
            _unitOfWork.OcopPartnerRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<List<PartnerRevenueDTO>> GetPartnerRevenue(int id, int? year)
        {
            var result = Enumerable.Range(1, 12)
        .Select(month => new PartnerRevenueDTO
        {
            Month = month,
            data = new List<RevenueDTO>
            {
                new RevenueDTO { orderStatus = Constants.Status.Canceled.ToString(), orderAmount = 0, amount = 0 },
                new RevenueDTO { orderStatus = Constants.Status.Completed.ToString(), orderAmount = 0, amount = 0 },
                new RevenueDTO { orderStatus = Constants.Status.Failed.ToString(), orderAmount = 0, amount = 0 },
                new RevenueDTO { orderStatus = Constants.Status.Shipping.ToString(), orderAmount = 0, amount = 0 },
                new RevenueDTO { orderStatus = Constants.Status.Pending.ToString(), orderAmount = 0, amount = 0 }
            }
        })
        .ToList();
            
            var partner = await _unitOfWork.OcopPartnerRepo.AnyAsync(u => u.Id == id);
            if (partner == false)
            {
                throw new KeyNotFoundException(ResponseMessage.PartnerIdNotFound);
            }
            var ordersQuery = await _unitOfWork.OrderRepo.GetQueryable(query => query.Where(u => u.OcopPartnerId.Equals(id)
                                                                                         && (u.Status == Constants.Status.Canceled
                                                                                         || u.Status == Constants.Status.Completed
                                                                                         || u.Status == Constants.Status.Shipping
                                                                                         || u.Status == Constants.Status.Failed
                                                                                         || u.Status == Constants.Status.Pending)
                                                                                         && u.IsDeleted == false)
                                                                                     .OrderBy(u => u.CreateDate));
            if (year.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.CreateDate.Value.Year == year.Value).ToList();

            }

            var groupedOrders = ordersQuery
        .GroupBy(o => new {
            Month = o.CreateDate.Value.Month,
            Status = o.Status.ToString()
        })
        .Select(g => new
        {
            Month = g.Key.Month,
            Status = g.Key.Status,
            OrderAmount = g.Count(),
            TotalAmount = g.Sum(o => o.TotalPriceAfterFee ?? 0)
        })
        .ToList();

            foreach (var monthGroup in result)
            {
                var monthOrders = groupedOrders.Where(g => g.Month == monthGroup.Month);

                foreach (var orderGroup in monthOrders)
                {
                    var statusEntry = monthGroup.data.FirstOrDefault(d =>
                        d.orderStatus == orderGroup.Status);

                    if (statusEntry != null)
                    {
                        statusEntry.orderAmount = orderGroup.OrderAmount;
                        statusEntry.amount = (int)orderGroup.TotalAmount;
                    }
                }
            }

            return result;
        }

        public async Task<PartnerProductDTO> GetPartnerTotalProduct(int id, int? storeId)
        {
            var lotsQuery = await _unitOfWork.LotRepo.GetQueryable(query => query.Where(u => u.Product.OcopPartnerId.Equals(id)
                                                                                          && u.ImportDate.HasValue
                                                                                          && u.RoomId.HasValue
                                                                                          && u.IsDeleted.Equals(false)
                                                                                          && (storeId == null || u.Room.StoreId.Equals(storeId)))
                                                                                 .Include(o => o.Room).ThenInclude(o => o.Store)
                                                                                 .Include(o => o.Product));
            lotsQuery = lotsQuery.ToList();

            var groupedProducts = lotsQuery
            .GroupBy(l => new
            {
                ProductId = l.ProductId,
                ProductName = l.Product.Name,
                ProductImage = l.Product.PictureLink,
                StoreId = l.Room.Store.Id,
                StoreName = l.Room.Store.Name,
                StoreLocation = l.Room.Store.Location + ", " + l.Room.Store.Province.SubDivisionName
            })
            .Select(group => new ProductDTO
            {
                id = (int)group.Key.ProductId,
                ProductName = group.Key.ProductName,
                ProductImage = group.Key.ProductImage,
                stock = group.Sum(l => (int)l.TotalProductAmount),
                storeId = (int)group.Key.StoreId,
                storeName = group.Key.StoreName,
                storeLocation = group.Key.StoreLocation
            })
            .OrderByDescending(p => p.stock)
            .ToList();
            int totalStock = groupedProducts.Sum(p => p.stock);
            return new PartnerProductDTO
            {
                totalProductAmount = totalStock,
                Products = groupedProducts
            };
        }

        public async Task<List<ProvinceListDTO>> GetProvince()
        {
            var list = await _unitOfWork.ProvinceRepo.GetAllAsync();
            var result = _mapper.Map<List<ProvinceListDTO>>(list);
            return result;
        }

        public async Task<string> CreatePartnerVerificationPaper(int ocop_partner_id, List<IFormFile> file)
        {
            if(file.Count == 0)
            {
                throw new ArgumentNullException();
            }

            var partner = await _unitOfWork.OcopPartnerRepo.AnyAsync(u => u.Id == ocop_partner_id);
            if(partner == false)
            {
                throw new KeyNotFoundException(ResponseMessage.PartnerIdNotFound);
            }
            var exist = await _unitOfWork.OcopPartnerVerificationPaperRepository.GetQueryable(u => u.Where(x => x.OcopPartnerId.Equals(ocop_partner_id)));
            exist = exist.ToList();
            if(exist.Count != 0)
            {
              if(exist.Any(x => x.IsVerified == 1))
              {
                   throw new ApplicationException("This account has already been verified.");
              }
              if(exist.Any(x => x.IsRejected == 0 && x.IsVerified == 0))
              {
                    throw new ApplicationException("You have to wait for your previous submission to be checked first.");
              }
            }
            string picture_link_1 = string.Empty;
            string picture_link_2 = string.Empty;

            foreach (var item in file)
            {
                if(picture_link_1 != string.Empty && picture_link_2 != string.Empty)
                {
                    break;
                }
                var picture_link = await _pictureService.UploadImage(item);
                if(picture_link_1 == string.Empty)
                {
                    picture_link_1 = picture_link;
                    continue;
                }
                if(picture_link_2 == string.Empty)
                {
                    picture_link_2 = picture_link;
                    continue;
                }
            }
            var request = new PartnerVerificationPaperCreateDTO
            {
                OcopPartnerId = ocop_partner_id
            };

            var result = _mapper.Map<OcopPartnerVerificationPaper>(request);
            result.FrontPictureLink = picture_link_1;
            result.BackPictureLink = picture_link_2;
            await _unitOfWork.OcopPartnerVerificationPaperRepository.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<PartnerVerificationPaperDTO> GetPartnerVerificationPaper(int partnerId)
        {
            var list = await _unitOfWork.OcopPartnerVerificationPaperRepository.GetQueryable(u => u.Where(x => x.OcopPartnerId == partnerId)
                                                                                                   .OrderBy(u => u.CreateDate));
            list = list.ToList();

            var partner = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Id.Equals(partnerId));

            var result = new PartnerVerificationPaperDTO
            {
                OCOPPartnerId = partnerId,
                OCOPPartnerEmail = partner?.Email, 
                IsVerified = partner?.IsVerified,
                data = list.Select(paper => new VerificationPaperDTO
                {
                    Id = paper.Id,
                    OcopPartnerId = paper.OcopPartnerId,
                    CreateDate = paper.CreateDate,
                    IsVerified = paper.IsVerified,
                    VerifyDate = paper.VerifyDate,
                    IsRejected = paper.IsRejected,
                    RejectDate = paper.RejectDate,
                    RejectReason = paper.RejectReason,
                    FrontPictureLink = paper.FrontPictureLink,
                    BackPictureLink = paper.BackPictureLink
                }).ToList()
            };
            return result;
        }

        public async Task<Pagination<PartnerVerificationPaperDTO>> GetAllPartnerVerificationPaper(bool? verified, int pageIndex, int pageSize)
        {
            
            var query = await _unitOfWork.OcopPartnerVerificationPaperRepository.GetQueryable(u => u.Include(o => o.OcopPartner));

            
            var groupedPapers = query
                .GroupBy(x => x.OcopPartner)  
                .Select(group => new
                 {
                    Partner = group.Key,  
                    Papers = group.OrderBy(p => p.CreateDate).ToList()
                });
            
            if (verified.HasValue)
            {
                groupedPapers = groupedPapers.Where(g =>
                    verified.Value
                        ? g.Papers.Any(p => p.IsVerified == 1)
                        : !g.Papers.Any(p => p.IsVerified == 1));
            }

            var items = groupedPapers.Select(group => new PartnerVerificationPaperDTO
            {
                OCOPPartnerId = group.Partner.Id,
                OCOPPartnerEmail = group.Partner.Email,
                IsVerified = group.Partner.IsVerified,
                data = group.Papers.Select(paper => new VerificationPaperDTO
                {
                    Id = paper.Id,
                    OcopPartnerId = paper.OcopPartnerId,
                    CreateDate = paper.CreateDate,
                    IsVerified = paper.IsVerified,
                    VerifyDate = paper.VerifyDate,
                    IsRejected = paper.IsRejected,
                    RejectDate = paper.RejectDate,
                    RejectReason = paper.RejectReason,
                    FrontPictureLink = paper.FrontPictureLink,
                    BackPictureLink = paper.BackPictureLink
                }).ToList()
            }).ToList();

            var result = _mapper.Map<List<PartnerVerificationPaperDTO>>(items);

            return await ListPagination<PartnerVerificationPaperDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<string> VerifyPartnerVerificationPaper(int partnerVerPaperid)
        {
            var exist = await _unitOfWork.OcopPartnerVerificationPaperRepository.SingleOrDefaultAsync(u => u.Id.Equals(partnerVerPaperid),
                                                                                                     query => query.Include(o => o.OcopPartner));
            if(exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OcopPartnerVerificationNotFound);
            }
            var partner = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.OcopPartnerId));

            exist.VerifyDate = DateTime.Now;
            exist.IsVerified = 1;
            exist.OcopPartner.IsVerified = 1;
            await _unitOfWork.SaveAsync();

            //send email here
            EmailSender(partner.Email, true);

            return ResponseMessage.Success;

        }

        public async Task<string> RejectPartnerVerificationPaper(int partnerVerPaperid, string reason)
        {
            if(reason == null || reason == string.Empty)
            {
                throw new ApplicationException(ResponseMessage.NoReasonGiven);
            }
            var exist = await _unitOfWork.OcopPartnerVerificationPaperRepository.SingleOrDefaultAsync(u => u.Id.Equals(partnerVerPaperid));
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.OcopPartnerVerificationNotFound);
            }
            if(exist.IsVerified == 1)
            {
                throw new ApplicationException(ResponseMessage.RejectVerifiedError);
            }
            exist.RejectDate = DateTime.Now;
            exist.IsRejected = 1;
            exist.RejectReason = reason;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private async void EmailSender(string targetMail, bool verify)
        {
            try
            {
                string saythis = string.Empty;
                if(verify == true)
                {
                    saythis = "Your account has been verified.";
                }
                else
                {
                    saythis = "Your submission has been rejected, please resubmit valid documents.";
                }
                var target = new MailAddress(targetMail);

                var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(Constants.DefaultString.systemJsonFile).Build();

                var mailConfig = config.GetSection("Mail").Get<AppConfiguration>();

                var keyVault = config.GetSection("KeyVault").Get<AppConfiguration>();

                var _client = new SecretClient(new Uri(keyVault.KeyVaultURL), new EnvironmentCredential());

                string smtpPassword = _client.GetSecret("BeeStore-Smtp-Password").Value.Value;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(mailConfig.sourceMail);
                mailMessage.Subject = Constants.Smtp.partnerVerification;
                mailMessage.To.Add(target);

                    mailMessage.Body = $@"
                                    <html>
                                      <body style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; border-radius: 10px;'>
                                          <h2 style='color: #4CAF50;'>Welcome to BeeShelf!</h2>
                                          <p>Dear User,</p>
                                          <p>{saythis}</p>
                                            <span> </span>
                                          
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

        public async Task<ProductStoreListDTO> GetProductByProvinceId(int productId, int provinceId, int partnerId)
        {
            var stores = await _unitOfWork.StoreRepo.GetQueryable(u => u.Where(x => x.ProvinceId.Equals(provinceId)
                                                                            && x.Rooms.Any(room => room.OcopPartnerId.Equals(partnerId)
                                                                                && room.Lots.Any(lot => lot.ProductId.Equals(productId) && lot.ImportDate.HasValue)))
                                                                        .Include(o => o.Rooms).ThenInclude(o => o.Lots).ThenInclude(o => o.Product)
                                                                        .Include(o => o.Province));
            stores = stores.ToList();
            var list = stores.Select(s => new StoreDTO
            {
                Id = s.Id,
                ProvinceId = s.ProvinceId,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Location = s.Location,
                ProvinceName = s.Province.SubDivisionName,
                ProductInStorage = s.Rooms
                .SelectMany(r => r.Lots)
                .Where(l => l.ProductId.Equals(productId))
                .Sum(l => l.TotalProductAmount)
            }).ToList();
            var totalProducts = list.Sum(s => s.ProductInStorage ?? 0);
            return new ProductStoreListDTO
            {
                totalProduct = totalProducts,
                data = list
            };
        }
    }
}
