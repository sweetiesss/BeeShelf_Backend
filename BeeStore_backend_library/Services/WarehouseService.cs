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
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public WarehouseService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CreateWarehouse(WarehouseCreateDTO request)
        {
            var exist = await _unitOfWork.WarehouseRepo.SingleOrDefaultAsync(u => u.Name == request.Name);
            if (exist != null)
            {
                throw new DuplicateException(ResponseMessage.WarehouseNameDuplicate);
            }
            var warehouse = _mapper.Map<Warehouse>(request);
            warehouse.CreateDate = DateTime.Now;
            await _unitOfWork.WarehouseRepo.AddAsync(warehouse);
            foreach(var o in request.DeliveryZones)
            {
                o.Name = o.Name.Trim();
                o.Location = o.Location.Trim();
                var deliveryzomama = await _unitOfWork.DeliveryZoneRepo.AnyAsync(u => (u.Name.Equals(o.Name, StringComparison.OrdinalIgnoreCase)
                                                                        || u.Location.Equals(o.Location, StringComparison.OrdinalIgnoreCase)) 
                                                                        && u.WarehouseId.Equals(warehouse.Id));
                if(deliveryzomama != false)
                {
                    throw new DuplicateException(ResponseMessage.DeliveryZoneDuplicateNameOrLocation);
                }
            }
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> AddDeliveryZonesToWarehouse(int warehouseId, List<DeliveryZoneCreateDTO> request)
        {
            var exist = await _unitOfWork.WarehouseRepo.SingleOrDefaultAsync(u => u.Id == warehouseId);
            if (exist == null)
            {
                throw new DuplicateException(ResponseMessage.WarehouseIdNotFound);
            }
            foreach (var o in request)
            {
                o.Name = o.Name.Trim();
                o.Location = o.Location.Trim();
                var deliveryzomama = await _unitOfWork.DeliveryZoneRepo.AnyAsync(u => (u.Name.Equals(o.Name, StringComparison.OrdinalIgnoreCase)
                                                                        || u.Location.Equals(o.Location, StringComparison.OrdinalIgnoreCase))
                                                                        && u.WarehouseId.Equals(warehouseId));
                if (deliveryzomama != false)
                {
                    throw new DuplicateException(ResponseMessage.DeliveryZoneDuplicateNameOrLocation);
                }
                o.WarehouseId = warehouseId;
            }
            var result = _mapper.Map<List<DeliveryZone>>(request);
            await _unitOfWork.DeliveryZoneRepo.AddRangeAsync(result);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> DeleteWarehouse(int id)
        {
            var exist = await _unitOfWork.WarehouseRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.WarehouseIdNotFound);
            }
            _unitOfWork.WarehouseRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private async Task<List<Warehouse>> ApplyFilterToList(string? search, WarehouseFilter? filterBy, string? filterQuery,
                                                         WarehouseSortBy? sortCriteria, bool descending)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }
            Expression<Func<Warehouse, bool>> filterExpression = u =>
            (filterBy == null || (filterBy == WarehouseFilter.WarehouseId && u.Id.Equals(Int32.Parse(filterQuery!))));


            string? sortBy = sortCriteria switch
            {
                WarehouseSortBy.Name => Constants.SortCriteria.Name,
                WarehouseSortBy.Size => Constants.SortCriteria.Size,
                WarehouseSortBy.Location => Constants.SortCriteria.Location,
                WarehouseSortBy.CreateDate => Constants.SortCriteria.CreateDate,
                _ => null
            };



            var list = await _unitOfWork.WarehouseRepo.GetListAsync(
                filter: filterExpression,
                includes: null,
                sortBy: sortBy!,
                descending: descending,
                searchTerm: search,
                searchProperties: new Expression<Func<Warehouse, string>>[] { p => p.Name }
                );
            return list;
        }

        public async Task<Pagination<WarehouseListDTO>> GetWarehouseList(string? search, WarehouseFilter? filterBy, string? filterQuery,
                                                         WarehouseSortBy? sortCriteria, bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(search, filterBy, filterQuery, sortCriteria, descending);
            var result = _mapper.Map<List<WarehouseListDTO>>(list);

            return await ListPagination<WarehouseListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<WarehouseDeliveryZoneDTO> GetWarehouseById(int id)
        {
            var warehouse = await _unitOfWork.WarehouseRepo.SingleOrDefaultAsync(w => !w.IsDeleted && w.Id == id,
                                                                          query => query.Include(w => w.DeliveryZones));

            var result = _mapper.Map<WarehouseDeliveryZoneDTO>(warehouse);
            return result;
        }

        public async Task<List<WarehouseListInventoryDTO>> GetWarehouseByUserId(int userId)
        {
            var list = await _unitOfWork.WarehouseRepo.GetQueryable(wh => wh
                                                       .Where(u => u.IsDeleted.Equals(false))
                                                       .Where(w => w.Inventories.Any(inventory => inventory.OcopPartnerId == userId))
        .Select(warehouse => new Warehouse
        {
            Id = warehouse.Id,
            Location = warehouse.Location,
            Name = warehouse.Name,
            Capacity = warehouse.Capacity,
            CreateDate = warehouse.CreateDate,
            Inventories = warehouse.Inventories.Where(inventory => inventory.OcopPartnerId == userId).ToList()
        }));
            var result = _mapper.Map<List<WarehouseListInventoryDTO>>(list);

            return result;
        }

        public async Task<string> UpdateWarehouse(int id, WarehouseCreateDTO request)
        {
            var exist = await _unitOfWork.WarehouseRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.WarehouseIdNotFound);
            }

            var duplicate = await _unitOfWork.WarehouseRepo.SingleOrDefaultAsync(u => u.Name.Equals(request.Name));
            if (duplicate != null && duplicate.Id != exist.Id)
            {
                throw new DuplicateException(ResponseMessage.WarehouseNameDuplicate);
            }

            if (!String.IsNullOrEmpty(request.Location) && !request.Location.Equals(Constants.DefaultString.String))
            {
                exist.Location = request.Location;
            }
            if (request.Capacity != null && request.Capacity != 0)
            {
                exist.Capacity = request.Capacity;
            }

            _unitOfWork.WarehouseRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;

        }

     
    }
}
