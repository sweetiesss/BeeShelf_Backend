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
        Task<Pagination<EmployeeListDTO>> GetAllEmployees(string search, EmployeeRole? role, UserSortBy? sortBy,
                                                 bool order, int pageIndex, int pageSize);
        Task<EmployeeListDTO> GetEmployee(string email);
        Task<string> CreateEmployee(EmployeeCreateRequest user);
        Task<string> UpdateEmployee(EmployeeUpdateRequest user);
        Task<string> DeleteEmployee(int id);

        Task<string> ForgotPassword(string email);
        Task<string> ResetPassword(UserForgotPasswordRequest request);
        Task<ManagerTotalRevenueDTO> GetManagerTotalRevenue(int warehouseId, int? year);
        Task<ManagerDashboardDTO> GetManagerDashboard(int? day, int? month, int? year);
    }
}
