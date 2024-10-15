using BeeStore_Repository.DTO.RoleDTOs;
using BeeStore_Repository.Interfaces;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleListDTO>> GetRoles();
        Task<string> UpdateUserRole(int id, string roleName);

    }
}
