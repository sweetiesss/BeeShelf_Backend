using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseDTOs;
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
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public StoreService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CreateStore(StoreCreateDTO request)
        {
            var exist = await _unitOfWork.StoreRepo.SingleOrDefaultAsync(u => u.Name == request.Name);
            if (exist != null)
            {
                throw new DuplicateException(ResponseMessage.WarehouseNameDuplicate);
            }
            var province = await _unitOfWork.ProvinceRepo.AnyAsync(u => u.Id.Equals(request.ProvinceId));
            if (province == false)
            {
                throw new KeyNotFoundException(ResponseMessage.ProvinceIdNotFound);
            }
            var warehouse = _mapper.Map<Store>(request);
            warehouse.CreateDate = DateTime.Now;
            await _unitOfWork.StoreRepo.AddAsync(warehouse);

            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }


        public async Task<string> DeleteStore(int id)
        {
            var exist = await _unitOfWork.StoreRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.WarehouseIdNotFound);
            }
            _unitOfWork.StoreRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private async Task<List<Store>> ApplyFilterToList(string? search, StoreFilter? filterBy, string? filterQuery,
                                                         StoreSortBy? sortCriteria, bool descending)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }
            Expression<Func<Store, bool>> filterExpression = u =>
            (filterBy == null ||
                (filterBy == StoreFilter.StoreId && u.Id.Equals(Int32.Parse(filterQuery!)))
             || (filterBy == StoreFilter.ProvinceId && u.Id.Equals(Int32.Parse(filterQuery!))));


            string? sortBy = sortCriteria switch
            {
                StoreSortBy.Name => Constants.SortCriteria.Name,
                StoreSortBy.Size => Constants.SortCriteria.Size,
                StoreSortBy.Location => Constants.SortCriteria.Location,
                StoreSortBy.CreateDate => Constants.SortCriteria.CreateDate,
                _ => null
            };



            var list = await _unitOfWork.StoreRepo.GetListAsync(
                filter: filterExpression,
                includes: u => u.Include(o => o.Province).Include(o => o.Rooms),
                sortBy: sortBy!,
                descending: descending,
                searchTerm: search,
                searchProperties: new Expression<Func<Store, string>>[] { p => p.Name }
                );
            return list;
        }

        public async Task<Pagination<StoreListDTO>> GetStoreList(string? search, StoreFilter? filterBy, string? filterQuery,
                                                         StoreSortBy? sortCriteria, bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(search, filterBy, filterQuery, sortCriteria, descending);
            var result = _mapper.Map<List<StoreListDTO>>(list);

            return await ListPagination<StoreListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<StoreDeliveryZoneDTO> GetStoreById(int id)
        {
            var warehouse = await _unitOfWork.StoreRepo.SingleOrDefaultAsync(w => !w.IsDeleted && w.Id == id,
                                                                          query => query.Include(w => w.Province).ThenInclude(o => o.DeliveryZones));

            var result = _mapper.Map<StoreDeliveryZoneDTO>(warehouse);
            return result;
        }

        public async Task<List<StoreListInventoryDTO>> GetStoreByUserId(int userId)
        {
            var list = await _unitOfWork.StoreRepo.GetQueryable(wh => wh
                                                       .Where(u => u.IsDeleted.Equals(false))
                                                       .Where(w => w.Rooms.Any(inventory => inventory.OcopPartnerId == userId))
            .Include(o => o.Province).Include(o => o.Rooms)
            .Select(warehouse => new Store
            {
                Id = warehouse.Id,
                Location = warehouse.Location,
                Name = warehouse.Name,
                Capacity = warehouse.Capacity,
                IsCold = warehouse.IsCold,
                Province = warehouse.Province,
                CreateDate = warehouse.CreateDate,
                Rooms = warehouse.Rooms.Where(inventory => inventory.OcopPartnerId == userId).ToList()
            }));
            var result = _mapper.Map<List<StoreListInventoryDTO>>(list);

            return result;
        }

        public async Task<string> UpdateStore(int id, StoreCreateDTO request)
        {
            var exist = await _unitOfWork.StoreRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.WarehouseIdNotFound);
            }

            var duplicate = await _unitOfWork.StoreRepo.SingleOrDefaultAsync(u => u.Name.Equals(request.Name));
            if (duplicate != null && duplicate.Id != exist.Id)
            {
                throw new DuplicateException(ResponseMessage.WarehouseNameDuplicate);
            }

            var province = await _unitOfWork.ProvinceRepo.AnyAsync(u => u.Id.Equals(request.ProvinceId));
            if (province == false)
            {
                throw new KeyNotFoundException(ResponseMessage.ProvinceIdNotFound);
            }
            exist.Name = request.Name;
            exist.Location = request.Location;
            exist.Capacity = request.Capacity;
            exist.IsCold = request.IsCold;
            exist.ProvinceId = request.ProvinceId;
            _unitOfWork.StoreRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;

        }


    }
}
