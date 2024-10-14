using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.UserDTOs;
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
        Task<UserListDTO> Login(string email, string password);
        Task<Pagination<UserListDTO>> GetAllUser(int pageIndex, int pageSize);
        Task<UserListDTO> GetUser(int id);
        Task<UserCreateRequestDTO> CreateUser(UserCreateRequestDTO user);
        Task<UserUpdateRequestDTO> UpdateUser(UserUpdateRequestDTO user);
        Task<string> DeleteUser(int id);
    }
}
