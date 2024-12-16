using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BeeStore_Repository.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly HashSet<string> _existingInventoryNames = new HashSet<string>();
        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        //NOT USED
        public async Task<string> AddPartnerToInventory(int id, int userId)
        {
            var exist = await _unitOfWork.InventoryRepo.GetByIdAsync(id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            if (exist.OcopPartnerId != null)
            {
                throw new DuplicateException(ResponseMessage.InventoryOccupied);
            }
            var user = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            if (user.RoleId != 4)
            {
                throw new KeyNotFoundException(ResponseMessage.UserRoleNotPartnerError);
            }

            exist.OcopPartnerId = user.Id;
            _unitOfWork.InventoryRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
        public async Task<string> CreateInventory(InventoryCreateDTO request)
        {
            var exist = await _unitOfWork.WarehouseRepo.GetByIdAsync(request.WarehouseId);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.WarehouseIdNotFound);
            }
            decimal? totalWeight = 0;
            if(request.InventoryAmount > 0)
            {
                totalWeight = request.InventoryAmount * request.MaxWeight;
            }
            else
            {
                throw new ApplicationException(ResponseMessage.BadRequest);
            }

            //Check inv capacity with total capacity of warehouse
            if ((totalWeight += exist.Inventories.Sum(o => o.MaxWeight).Value) > exist.Capacity)
            {
                throw new ApplicationException(ResponseMessage.WarehouseAddInventoryCapacityLimitReach);
            }
            for (int i = 0; i < request.InventoryAmount; i++)
            {
                string inventoryName;
                int attempts = 0;
                const int maxAttempts = 100; 

                do
                {
                    inventoryName = GenerateUniqueInventoryName();
                    attempts++;

                    if (attempts >= maxAttempts)
                    {
                        throw new ApplicationException();
                    }
                }
                while (_existingInventoryNames.Contains(inventoryName) ||
                       exist.Inventories.Any(inv => inv.Name == inventoryName && inv.WarehouseId.Equals(request.WarehouseId)));

                
                _existingInventoryNames.Add(inventoryName);

                exist.Inventories.Add(new Inventory
                {
                    Name = inventoryName,
                    MaxWeight = request.MaxWeight,
                    Weight = 0,
                    Price = request.Price,
                    WarehouseId = request.WarehouseId,
                });
            }


            //_unitOfWork.WarehouseRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> DeleteInventory(int id)
        {
            var exist = await _unitOfWork.InventoryRepo.GetByIdAsync(id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            _unitOfWork.InventoryRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> UpdateInventory(InventoryUpdateDTO request)
        {

            var exist = await _unitOfWork.InventoryRepo.GetByIdAsync(request.Id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            if (DateTime.Now > exist.BoughtDate && DateTime.Now < exist.ExpirationDate)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryOccupied);
            }
            if (request.MaxWeight != null && request.MaxWeight != 0)
            {
                exist.MaxWeight = request.MaxWeight;
            }
            exist.Price = request.Price;
            _unitOfWork.InventoryRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;

        }

        private async Task<List<Inventory>> ApplyFilterToList(InventoryFilter? filterBy, string? filterQuery,
                                                          InventorySortBy? sortCriteria,
                                                          bool descending, int? userId = null)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }
            Expression<Func<Inventory, bool>> filterExpression = u =>
            (userId == null || u.OcopPartnerId.Equals(userId)) &&
            (filterBy == null || (filterBy == InventoryFilter.WarehouseId && u.WarehouseId.Equals(Int32.Parse(filterQuery!))));


            string? sortBy = sortCriteria switch
            {
                InventorySortBy.BoughtDate => Constants.SortCriteria.BoughtDate,
                InventorySortBy.ExpirationDate => Constants.SortCriteria.ExpirationDate,
                InventorySortBy.Name => Constants.SortCriteria.Name,
                InventorySortBy.MaxWeight => Constants.SortCriteria.MaxWeight,
                InventorySortBy.Weight => Constants.SortCriteria.Weight,
                _ => null
            };

            var list = await _unitOfWork.InventoryRepo.GetListAsync(
                filter: filterExpression,
                includes: u => u.Include(o => o.Lots),
                sortBy: sortBy!,
                descending: descending,
                searchTerm: null,
                searchProperties: null
                );
            return list;
        }

        public async Task<Pagination<InventoryListDTO>> GetInventoryList(InventoryFilter? filterBy, string filterQuery,
                                                          InventorySortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(filterBy, filterQuery, sortCriteria, descending);
            var result = _mapper.Map<List<InventoryListDTO>>(list);
            return (await ListPagination<InventoryListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<Pagination<InventoryListDTO>> GetInventoryList(int userId, InventoryFilter? filterBy, string filterQuery,
                                                          InventorySortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(filterBy, filterQuery, sortCriteria, descending, userId);


            var result = _mapper.Map<List<InventoryListDTO>>(list);
            return (await ListPagination<InventoryListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<InventoryLotListDTO> GetInventoryById(int id)
        {
            var exist = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            var result = _mapper.Map<InventoryLotListDTO>(exist);
            return result;
        }

        public async Task<string> BuyInventory(int id, int userId, int month)
        {
            if (month <= 0)
            {
                throw new ApplicationException(ResponseMessage.BadRequest);
            }
            var user = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Id == userId,
                                                                              query => query.Include(o => o.Wallets));
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inv = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (inv == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            //m them cai || (DateTime.Now > inv.BoughtDate && DateTime.Now < inv.ExpirationDate) nay do no bi loi nen t xoa di r.
            if (inv.OcopPartnerId != null && inv.OcopPartnerId != userId)
            {
                throw new ApplicationException(ResponseMessage.InventoryOccupied);
            }
            var wallet = user.Wallets.FirstOrDefault(u => u.OcopPartnerId == userId);
            if (wallet.TotalAmount < inv.Price * month)
            {
                throw new ApplicationException(ResponseMessage.NotEnoughCredit);
            }
            wallet.TotalAmount -= inv.Price * month;
            inv.OcopPartnerId = userId;
            inv.BoughtDate = DateTime.Now;
            inv.ExpirationDate = DateTime.Now.AddMonths(month);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> ExtendInventory(int id, int userId, int month)
        {
            if (month == 0)
            {
                throw new ApplicationException(ResponseMessage.BadRequest);
            }
            var user = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Id == userId,
                                                                              query => query.Include(o => o.Wallets));
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inv = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (inv == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            if (inv.OcopPartnerId != userId)
            {
                throw new ApplicationException(ResponseMessage.InventoryPartnerNotMatch);
            }
            var wallet = user.Wallets.FirstOrDefault(u => u.OcopPartnerId == userId);
            if (wallet.TotalAmount < inv.Price * month)
            {
                throw new ApplicationException(ResponseMessage.NotEnoughCredit);
            }
            inv.ExpirationDate = inv.ExpirationDate.Value.AddMonths(month);
            wallet.TotalAmount -= inv.Price * month;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private string GenerateUniqueInventoryName()
        {
            // Generate a random 5-digit number
            return $"INV-{Random.Shared.Next(10000, 99999):D5}";
        }
    }
}
