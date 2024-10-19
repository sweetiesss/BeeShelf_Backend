using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.SortBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserLoginResponseDTO> Login(string email, string password);
        Task<string> SignUp(UserSignUpRequestDTO request);
        Task<Pagination<UserListDTO>> GetAllUser(string search, UserRole? role, UserSortBy? sortBy,
                                                 bool order, int pageIndex, int pageSize);
        Task<UserListDTO> GetUser(string email);
        Task<string> CreateUser(UserCreateRequestDTO user);
        Task<string> UpdateUser(UserUpdateRequestDTO user);
        Task<string> DeleteUser(int id);
    }
}
