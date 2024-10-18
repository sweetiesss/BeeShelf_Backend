using BeeStore_Repository.DTO.UserDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IJWTService
    {
        string RefreshJWTToken(UserRefreshTokenRequestDTO jwt);
        string GenerateJwtToken(string userId, string userRole);
    }
}
