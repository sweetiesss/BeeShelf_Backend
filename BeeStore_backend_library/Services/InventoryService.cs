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
using System.Linq.Expressions;

namespace BeeStore_Repository.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
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
            //uncomment these after database changes
            //var dupe = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Name.Equals(request.Name,
            //                                                                StringComparison.OrdinalIgnoreCase)
            //                                                                && u.WarehouseId.Equals(request.WarehouseId));
            //if (dupe != null)
            //{
            //    throw new ApplicationException(ResponseMessage.InventoryNameDuplicate);
            //}
            var result = _mapper.Map<Inventory>(request);
            result.BoughtDate = DateTime.Now;
            result.ExpirationDate = DateTime.Now;
            await _unitOfWork.InventoryRepo.AddAsync(result);
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
            //var dupe = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Name.Equals(request.Name,
            //                                                                StringComparison.OrdinalIgnoreCase)
            //                                                                && u.WarehouseId.Equals(exist.WarehouseId));
            //if(dupe != null && !dupe.Id.Equals(request.Id))
            // {
            //        throw new ApplicationException(ResponseMessage.InventoryNameDuplicate);
            // }

            if (request.Name != null && !request.Name.Equals(Constants.DefaultString.String))
            {//uncomment these after you make changes to database
                //exist.Name = request.Name;
            }
            if (request.Weight != null)
            {
                //exist.Weight = request.Weight;
            }
            if (request.MaxWeight != null && request.MaxWeight != 0)
            {
                exist.MaxWeight = request.MaxWeight;
            }
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
                includes: null,
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
    }
}
