﻿using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text;

namespace BeeStore_Repository.Services
{
    public class StoreShipperService : IStoreShipperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StoreShipperService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private async Task<List<StoreShipper>> ApplyFilterToList(string? search,bool? hasDeliveryZone, bool? hasVehicle, StoreFilter? filterBy, string? filterQuery, int? warehouseId = null)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }
            Expression<Func<StoreShipper, bool>> filterExpression = u =>
            (warehouseId == null || u.StoreId.Equals(warehouseId)) &&
            (filterBy == null ||
                (filterBy == StoreFilter.StoreId && u.StoreId.Equals(Int32.Parse(filterQuery!))) ||
                (filterBy == StoreFilter.DeliveryZoneId && u.DeliveryZoneId.Equals(Int32.Parse(filterQuery!))))
                && (hasVehicle == null ||
        (hasVehicle == true && u.Employee.Vehicles.Count() > 0) ||
        (hasVehicle == false && u.Employee.Vehicles.Count() == 0))
                && (hasDeliveryZone == null ||
        (hasDeliveryZone == true && u.Employee.StoreShippers.First().DeliveryZoneId.HasValue) ||
        (hasDeliveryZone == false && !u.Employee.StoreShippers.First().DeliveryZoneId.HasValue));


            var list = await _unitOfWork.StoreShipperRepo.GetListAsync(
                filter: filterExpression,
                includes: u => u.Include(o => o.Employee).ThenInclude(o => o.Vehicles)
                                .Include(o => o.Store).Include(o => o.DeliveryZone),
                sortBy: null,
                descending: false,
                searchTerm: search,
                searchProperties: new Expression<Func<StoreShipper, string>>[] { p => p.Employee.Email }
                );
            return list;
        }

        public async Task<Pagination<StoreShipperListDTO>> GetStoreShipperList(string? search, bool? hasDeliveryZone, bool? hasVehicle, StoreFilter? filterBy, string? filterQuery, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(search,hasDeliveryZone, hasVehicle, filterBy, filterQuery);
            var result = _mapper.Map<List<StoreShipperListDTO>>(list);
            return (await ListPagination<StoreShipperListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<Pagination<StoreShipperListDTO>> GetStoreShipperList(int id, string? search, bool? hasDeliveryZone, bool? hasVehicle, StoreFilter? filterBy, string? filterQuery, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(search,hasDeliveryZone, hasVehicle, filterBy, filterQuery, id);
            var result = _mapper.Map<List<StoreShipperListDTO>>(list);
            return (await ListPagination<StoreShipperListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<string> AddShipperToStore(List<StoreShipperCreateDTO> request)
        {
            StringBuilder sb = new StringBuilder();
            string error = string.Empty;

            //check dupes in list
            var dupes = request.Select((x, i) => new { index = i, value = x })
                   .GroupBy(x => new { x.value.EmployeeId })
                   .Where(x => x.Skip(1).Any());

            if (dupes.Any())
            {
                foreach (var group in dupes)
                {
                    var a = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id == group.Key.EmployeeId);
                    sb.Append($"{a.Email}, ");
                }
                error = sb.ToString();
                if (!String.IsNullOrEmpty(error))
                {
                    throw new DuplicateException(ResponseMessage.WarehouseUserDuplicateList + error);
                }
            }

            foreach (var item in request)
            {
                //check user exist and is a shipper
                var user = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id == item.EmployeeId);

                if (user == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.UserIdNotFound + $": {item.EmployeeId}");
                }

                var role = await _unitOfWork.RoleRepo.SingleOrDefaultAsync(u => u.Id == user.RoleId);
                if (role.RoleName != Constants.RoleName.Shipper)
                {
                    throw new AppException(ResponseMessage.UserRoleNotShipperError + $": {user.Email}");
                }

                //check if user is arleady working at here or another place
                var existWorking = await _unitOfWork.StoreShipperRepo.FirstOrDefaultAsync(u => u.EmployeeId == item.EmployeeId);
                if (existWorking != null)
                {
                    if (existWorking.IsDeleted != true)
                    {
                        //throw new DuplicateException($"This user is already working at a warehouse: ID: {existWorking.User.Id}");
                        sb.Append($"{existWorking.EmployeeId}, ");

                    }
                    else
                    {
                        existWorking.EmployeeId = null;
                        _unitOfWork.StoreShipperRepo.Update(existWorking);
                        await _unitOfWork.SaveAsync();
                    }
                }
            }
            error = sb.ToString();
            if (!String.IsNullOrEmpty(error))
            {
                throw new DuplicateException(ResponseMessage.WarehouseUserAddListFailed + error);
            }
            var result = _mapper.Map<List<StoreShipper>>(request);
            await _unitOfWork.StoreShipperRepo.AddRangeAsync(result);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> AssignShipperToDeliveryZone(int shipperId, int deliveryZoneId)
        {
            var shipper = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id.Equals(shipperId),
                                                                              query => query
                                                                              .Include(o => o.StoreShippers)
                                                                              .ThenInclude(o => o.Store)
                                                                              .Include(o => o.Role));
            if (shipper == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            if (shipper.Role.RoleName != Constants.RoleName.Shipper)
            {
                throw new ApplicationException(ResponseMessage.UserRoleNotShipperError);
            }
            var deliveryZone = await _unitOfWork.DeliveryZoneRepo.SingleOrDefaultAsync(u => u.Id.Equals(deliveryZoneId));
            if (deliveryZoneId == null)
            {
                throw new KeyNotFoundException(ResponseMessage.DeliveryZoneIdNotFound);
            }
            if (deliveryZone.ProvinceId !=
                shipper.StoreShippers.FirstOrDefault(u => u.EmployeeId.Equals(shipperId)
                                                        && u.IsDeleted.Equals(false))
                                                        .Store
                                                        .ProvinceId)
            {
                throw new ApplicationException(ResponseMessage.DeliveryZoneProvinceIdNotMatchWithShipper);
            }
            var result = shipper.StoreShippers.FirstOrDefault(u => u.EmployeeId.Equals(shipperId)
                                                                 && u.IsDeleted.Equals(false));
            result.DeliveryZoneId = deliveryZoneId;
            result.Status = "Active";
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;

        }
    }
}
