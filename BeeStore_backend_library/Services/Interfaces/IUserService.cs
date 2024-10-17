using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.UserDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserListDTO> Login(string email, string password);
        Task<string> SignUp(UserSignUpRequestDTO request);
        Task<Pagination<UserListDTO>> GetAllUser(int pageIndex, int pageSize);
        Task<UserListDTO> GetUser(string email);
        Task<string> CreateUser(UserCreateRequestDTO user);
        Task<string> UpdateUser(UserUpdateRequestDTO user);
        Task<string> DeleteUser(int id);
    }
}
