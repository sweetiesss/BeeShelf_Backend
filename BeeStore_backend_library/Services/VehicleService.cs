﻿using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.VehicleDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeeStore_Repository.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public VehicleService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> AssignVehicle(int id, int driver_id)
        {
            var employee = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id.Equals(driver_id),
                                                                              query => query.Include(o => o.Vehicles));
            if (employee == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            if (employee.RoleId != 4)
            {
                throw new ApplicationException(ResponseMessage.UserRoleNotShipperError);
            }
            var list = employee.Vehicles.ToList();
            foreach (var x in list)
            {
                x.AssignedDriverId = null;
            }
            var vehicle = await _unitOfWork.VehicleRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (vehicle == null)
            {
                throw new KeyNotFoundException(ResponseMessage.VehicleIdNotFound);
            }
            vehicle.AssignedDriverId = driver_id;
            //vehicle.Status = Constants.VehicleStatus.InService;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> UnassignVehicle(int id)
        {
            var vehicle = await _unitOfWork.VehicleRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (vehicle == null)
            {
                throw new KeyNotFoundException(ResponseMessage.VehicleIdNotFound);
            }
            if (vehicle.Status.Equals(Constants.VehicleStatus.InService))
            {
                throw new ApplicationException(ResponseMessage.VehicleCurrentlyInService);
            }
            vehicle.AssignedDriverId = null;
            _unitOfWork.VehicleRepo.Update(vehicle);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> CreateVehicle(VehicleType? type, VehicleCreateDTO request)
        {
            var vehicle = await _unitOfWork.VehicleRepo.AnyAsync(u => u.LicensePlate == request.LicensePlate);
            var warehouse = await _unitOfWork.StoreRepo.SingleOrDefaultAsync(o => o.Id.Equals(request.StoreId));
            if (warehouse == null)
            {
                throw new KeyNotFoundException(ResponseMessage.WarehouseIdNotFound);
            }

            if (vehicle != false)
            {
                throw new DuplicateException(ResponseMessage.VehicleLicensePlateDuplicate);
            }
            string VeType = string.Empty;
            switch (type)
            {
                case VehicleType.Van:
                    VeType = Constants.VehicleType.Van;
                    if(request.Capacity < 500 || request.Capacity > 3500)
                    {
                        throw new ApplicationException("Van's capacity should range from 500kg - 3500kg");
                    }
                    break;
                case VehicleType.Truck:
                    VeType = Constants.VehicleType.Truck;
                    if(request.Capacity < 1000 || request.Capacity > 30000)
                    {
                        throw new ApplicationException("Truck's capacity should range from 1000kg - 30000kg");
                    }
                    break;
                case VehicleType.Motorcycle:
                    VeType = Constants.VehicleType.Motorcycle;
                    if (request.Capacity < 50 || request.Capacity > 200)
                    {
                        throw new ApplicationException("Motorcycle's capacity should range from 50kg - 200kg");
                    }
                    break;
                default:
                    break;
            };

            if (VeType == string.Empty)
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }
            request.Type = VeType;
            request.Status = Constants.VehicleStatus.Available;
            var result = _mapper.Map<Vehicle>(request);
            result.Capacity = request.Capacity;
            await _unitOfWork.VehicleRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;

        }


        public async Task<string> DeleteVehicle(int id)
        {
            var vehicle = await _unitOfWork.VehicleRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (vehicle == null)
            {
                throw new KeyNotFoundException(ResponseMessage.VehicleIdNotFound);
            }
            if (vehicle.Status.Equals(Constants.VehicleStatus.InService))
            {
                throw new ApplicationException(ResponseMessage.VehicleCurrentlyInService);
            }
            _unitOfWork.VehicleRepo.SoftDelete(vehicle);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<VehicleListDTO> GetShipperVehicle(int shipperId)
        {
            var vehicle = await _unitOfWork.VehicleRepo.SingleOrDefaultAsync(u => u.AssignedDriverId.Equals(shipperId));
            if (vehicle == null)
            {
                throw new KeyNotFoundException(ResponseMessage.VehicleIdNotFound);
            }
            var result = _mapper.Map<VehicleListDTO>(vehicle);
            return result;
        }

        public async Task<VehicleListDTO> GetVehicle(int id)
        {
            var vehicle = await _unitOfWork.VehicleRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (vehicle == null)
            {
                throw new KeyNotFoundException(ResponseMessage.VehicleIdNotFound);
            }
            var result = _mapper.Map<VehicleListDTO>(vehicle);
            return result;
        }

        public async Task<Pagination<VehicleListDTO>> GetVehicles(VehicleStatus? status, VehicleType? type, VehicleSortBy? sortCriteria, bool descending, int pageIndex, int pageSize, int? warehouseId)
        {


            string? vestatus = status switch
            {
                VehicleStatus.Available => Constants.VehicleStatus.Available,
                VehicleStatus.InService => Constants.VehicleStatus.InService,
                VehicleStatus.Repair => Constants.VehicleStatus.Repair,
                _ => null
            };

            string? vetype = type switch
            {
                VehicleType.Truck => Constants.VehicleType.Truck,
                VehicleType.Van => Constants.VehicleType.Van,
                VehicleType.Motorcycle => Constants.VehicleType.Motorcycle,
                _ => null
            };



            string? sortBy = sortCriteria switch
            {
                VehicleSortBy.Capacity => Constants.SortCriteria.Capacity,
                _ => null
            };

            var list = await _unitOfWork.VehicleRepo.GetListAsync(
                filter: u => (vestatus == null || u.Status.Equals(vestatus))
                             && (vetype == null || u.Type.Equals(vetype))
                             && (warehouseId == null || u.StoreId.Equals(warehouseId)),
                includes: u => u.Include(o => o.AssignedDriver),
                sortBy: sortBy!,
                descending: descending,
                searchTerm: null,
                searchProperties: null
                );
            var result = _mapper.Map<List<VehicleListDTO>>(list);

            return await ListPagination<VehicleListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<string> UpdateVehicle(int id, VehicleType? type, VehicleCreateDTO request)
        {
            var vehicle = await _unitOfWork.VehicleRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (vehicle == null)
            {
                throw new KeyNotFoundException(ResponseMessage.VehicleIdNotFound);
            }
            if (vehicle.Status.Equals(Constants.VehicleStatus.InService))
            {
                throw new ApplicationException(ResponseMessage.VehicleCurrentlyInService);
            }
            var LPexist = await _unitOfWork.VehicleRepo.AnyAsync(u => u.LicensePlate == request.LicensePlate && u.Id != vehicle.Id);
            if (LPexist != false)
            {
                throw new DuplicateException(ResponseMessage.VehicleLicensePlateDuplicate);
            }


            string VeType = string.Empty;
            switch (type)
            {
                case VehicleType.Van:
                    VeType = Constants.VehicleType.Van;
                    break;
                case VehicleType.Truck:
                    VeType = Constants.VehicleType.Truck;
                    break;
                case VehicleType.Motorcycle:
                    VeType = Constants.VehicleType.Motorcycle;
                    break;
                default:
                    break;
            };

            if (VeType != string.Empty)
            {
                vehicle.Type = VeType;
            }
            vehicle.Name = request.Name;
            vehicle.LicensePlate = request.LicensePlate;
            vehicle.StoreId = request.StoreId;
            vehicle.Capacity = request.Capacity;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> UpdateVehicleStatus(int id, VehicleStatus? status)
        {
            var vehicle = await _unitOfWork.VehicleRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (vehicle == null)
            {
                throw new KeyNotFoundException(ResponseMessage.VehicleIdNotFound);
            }
            string? veStat = status switch
            {
                VehicleStatus.InService => Constants.VehicleStatus.InService,
                VehicleStatus.Repair => Constants.VehicleStatus.Repair,
                VehicleStatus.Available => Constants.VehicleStatus.Available,
                _ => null
            };
            if (veStat == null)
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }
            if (veStat.Equals(Constants.VehicleStatus.Repair))
            {
                if (vehicle.AssignedDriver != null)
                {
                    throw new BadHttpRequestException(ResponseMessage.VehicleAssigned);
                }
                if (vehicle.Status.Equals(Constants.VehicleStatus.InService))
                {
                    throw new ApplicationException(ResponseMessage.VehicleCurrentlyInService);
                }
            }
            vehicle.Status = veStat;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
    }
}
