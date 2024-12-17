using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.DTO.ProvinceDTOs;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BeeStore_Repository.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public PartnerService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
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
            //list.Add(new PartnerRevenueDTO
            //{
            //    orderStatus = Constants.Status.Canceled,
            //    orderAmount = 0,
            //    amount = 0,
            //});
            //list.Add(new PartnerRevenueDTO
            //{
            //    orderStatus = Constants.Status.Completed,
            //    orderAmount = 0,
            //    amount = 0,
            //});
            //list.Add(new PartnerRevenueDTO
            //{
            //    orderStatus = Constants.Status.Failed,
            //    orderAmount = 0,
            //    amount = 0,
            //});
            //list.Add(new PartnerRevenueDTO
            //{
            //    orderStatus = Constants.Status.Shipping,
            //    orderAmount = 0,
            //    amount = 0,
            //});
            //list.Add(new PartnerRevenueDTO
            //{
            //    orderStatus = Constants.Status.Pending,
            //    orderAmount = 0,
            //    amount = 0,
            //});
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
                //if (month.HasValue)
                //{
                //    ordersQuery = ordersQuery.Where(o => o.CreateDate.Value.Month == month.Value).ToList();
                //    if (day.HasValue)
                //    {
                //        ordersQuery = ordersQuery.Where(o => o.CreateDate.Value.Day == day.Value).ToList();
                //    }
                //}
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
            TotalAmount = g.Sum(o => o.TotalPrice ?? 0)
        })
        .ToList();

            //var groupedOrders = ordersQuery
            //.GroupBy(o => o.Status)
            //.Select(g => new
            //{
            //    Status = g.Key,
            //    OrderAmount = g.Count(),
            //    TotalAmount = g.Sum(o => o.TotalPrice ?? 0)
            //});

            //foreach (var group in groupedOrders)
            //{
            //    var dto = list.FirstOrDefault(d => d.orderStatus == group.Status);
            //    if (dto != null)
            //    {
            //        dto.orderAmount = group.OrderAmount;
            //        dto.amount = (int)group.TotalAmount;
            //    }
            //}
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

        public async Task<PartnerProductDTO> GetPartnerTotalProduct(int id, int? warehouseId)
        {
            var lotsQuery = await _unitOfWork.LotRepo.GetQueryable(query => query.Where(u => u.Product.OcopPartnerId.Equals(id)
                                                                                          && u.ImportDate.HasValue
                                                                                          && u.InventoryId.HasValue
                                                                                          && u.IsDeleted.Equals(false)
                                                                                          && (warehouseId == null || u.Inventory.WarehouseId.Equals(warehouseId)))
                                                                                 .Include(o => o.Inventory).ThenInclude(o => o.Warehouse)
                                                                                 .Include(o => o.Product));
            lotsQuery = lotsQuery.ToList();

            var groupedProducts = lotsQuery
            .GroupBy(l => new
            {
                ProductId = l.ProductId,
                ProductName = l.Product.Name,
                WarehouseId = l.Inventory.Warehouse.Id,
                WarehouseName = l.Inventory.Warehouse.Name,
                WarehouseLocation = l.Inventory.Warehouse.Location + ", " + l.Inventory.Warehouse.Province.SubDivisionName
            })
            .Select(group => new ProductDTO
            {
                id = (int)group.Key.ProductId,
                ProductName = group.Key.ProductName,
                stock = group.Sum(l => (int)l.TotalProductAmount),
                warehouseId = (int)group.Key.WarehouseId,
                warehouseName = group.Key.WarehouseName,
                warehouseLocation = group.Key.WarehouseLocation
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
    }
}
