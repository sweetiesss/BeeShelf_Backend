using BeeStore_Repository.DTO;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IUserService
    {
        Task<Pagination<UserListDTO>> GetAllUser(int pageIndex, int pageSize);
        Task<UserListDTO> Login(string email, string password);
    }
}
