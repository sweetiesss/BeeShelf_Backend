namespace BeeStore_Repository.Services.Interfaces
{
    public interface IJWTService
    {
        string GenerateJwtToken(string userId, string userRole);
    }
}
