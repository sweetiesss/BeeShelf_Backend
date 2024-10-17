using BeeStore_Repository.DTO.RoleDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleListDTO>> GetRoles();
        Task<string> UpdateUserRole(int id, string roleName);

    }
}
