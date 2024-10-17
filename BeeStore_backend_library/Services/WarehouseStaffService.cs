using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
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

        public async Task<List<WarehouseStaffCreateDTO>> AddStaffToWarehouse(List<WarehouseStaffCreateDTO> request)
        {
            StringBuilder sb = new StringBuilder();
            string error = string.Empty;

            //check dupes in list
            var dupes = request.Select((x, i) => new { index = i, value = x })
                   .GroupBy(x => new { x.value.UserId })
                   .Where(x => x.Skip(1).Any());

            if (dupes.Any())
            {
                foreach (var group in dupes)
                {
                    var a = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == group.Key.UserId);
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
                var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == item.UserId);

                if (user == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound + $": {item.UserId}");
                }

                var role = await _unitOfWork.RoleRepo.SingleOrDefaultAsync(u => u.Id == user.RoleId);
                if (role.RoleName != Constants.RoleName.Staff)
                {
                    throw new AppException(ResponseMessage.UserRoleNotStaffError + $": {user.Email}");
                }

                //check if user is arleady working at here or another place
                var existWorking = await _unitOfWork.WarehouseStaffRepo.FirstOrDefaultAsync(u => u.UserId == item.UserId);
                if (existWorking != null)
                {
                    if (existWorking.IsDeleted != true)
                    {
                        sb.Append($"{existWorking.User.Id}, ");
                    }
                    else
                    {
                        existWorking.UserId = null;
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
            return request;

        }

        public async Task<Pagination<WarehouseStaffListDTO>> GetWarehouseStaffList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.WarehouseStaffRepo.GetAllAsync();
            var result = _mapper.Map<List<WarehouseStaffListDTO>>(list);
            return (await ListPagination<WarehouseStaffListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<Pagination<WarehouseStaffListDTO>> GetWarehouseStaffList(int id, int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.WarehouseStaffRepo.GetFiltered(u => u.WarehouseId == id);
            var result = _mapper.Map<List<WarehouseStaffListDTO>>(list);
            return (await ListPagination<WarehouseStaffListDTO>.PaginateList(result, pageIndex, pageSize));
        }
    }
}
