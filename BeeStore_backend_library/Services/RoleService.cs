using AutoMapper;
using BeeStore_Repository.DTO.RoleDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;

namespace BeeStore_Repository.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public RoleService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<RoleListDTO>> GetRoles()
        {
            var list = await _unitOfWork.RoleRepo.GetAllAsync();
            var result = _mapper.Map<List<RoleListDTO>>(list);
            return result;
        }

        public async Task<string> UpdateUserRole(int id, string roleName)
        {
            var user = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var role = await _unitOfWork.RoleRepo.SingleOrDefaultAsync(u => u.RoleName
                                                 .Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (role == null || roleName.Equals(Constants.RoleName.Admin, StringComparison.OrdinalIgnoreCase))
            {
                throw new KeyNotFoundException(ResponseMessage.RoleNotFound);
            }
            user.RoleId = role.Id;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
    }
}
