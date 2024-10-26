using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Text;

namespace BeeStore_Repository.Services
{
    public class WarehouseStaffService : IWarehouseStaffService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WarehouseStaffService(UnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> AddStaffToWarehouse(List<WarehouseStaffCreateDTO> request)
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
                //check user exist and is a staff
                var user = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id == item.EmployeeId);

                if (user == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound + $": {item.EmployeeId}");
                }

                var role = await _unitOfWork.RoleRepo.SingleOrDefaultAsync(u => u.Id == user.RoleId);
                if (role.RoleName != Constants.RoleName.Staff)
                {
                    throw new AppException(ResponseMessage.UserRoleNotStaffError + $": {user.Email}");
                }

                //check if user is arleady working at here or another place
                var existWorking = await _unitOfWork.WarehouseStaffRepo.FirstOrDefaultAsync(u => u.EmployeeId == item.EmployeeId);
                if (existWorking != null)
                {
                    if (existWorking.IsDeleted != true)
                    {
                        sb.Append($"{existWorking.Employee.Id}, ");
                    }
                    else
                    {
                        existWorking.EmployeeId = null;
                        _unitOfWork.WarehouseStaffRepo.Update(existWorking);
                        await _unitOfWork.SaveAsync();
                    }
                }
            }
            error = sb.ToString();
            if (!String.IsNullOrEmpty(error))
            {
                throw new DuplicateException(ResponseMessage.WarehouseUserAddListFailed + error);
            }
            var result = _mapper.Map<List<WarehouseStaff>>(request);
            await _unitOfWork.WarehouseStaffRepo.AddRangeAsync(result);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;

        }

        private async Task<List<WarehouseStaff>> ApplyFilterToList(string? search, WarehouseFilter? filterBy, string? filterQuery, int? warehouseId = null)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }
            Expression<Func<WarehouseStaff, bool>> filterExpression = u =>
            (warehouseId == null || u.WarehouseId.Equals(warehouseId)) &&
            (filterBy == null || (filterBy == WarehouseFilter.WarehouseId && u.WarehouseId.Equals(Int32.Parse(filterQuery!))));


            var list = await _unitOfWork.WarehouseStaffRepo.GetListAsync(
                filter: filterExpression,
                includes: null,
                sortBy: null,
                descending: false,
                searchTerm: search,
                searchProperties: new Expression<Func<WarehouseStaff, string>>[] { p => p.Employee.Email }
                );
            return list;
        }

        public async Task<Pagination<WarehouseStaffListDTO>> GetWarehouseStaffList(string? search, WarehouseFilter? filterBy, string? filterQuery, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(search, filterBy, filterQuery);
            var result = _mapper.Map<List<WarehouseStaffListDTO>>(list);
            return (await ListPagination<WarehouseStaffListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<Pagination<WarehouseStaffListDTO>> GetWarehouseStaffList(int id, string? search, WarehouseFilter? filterBy, string? filterQuery, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(search, filterBy, filterQuery, id);
            var result = _mapper.Map<List<WarehouseStaffListDTO>>(list);
            return (await ListPagination<WarehouseStaffListDTO>.PaginateList(result, pageIndex, pageSize));
        }
    }
}
