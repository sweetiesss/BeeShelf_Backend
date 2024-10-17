using Microsoft.AspNetCore.Http;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IPictureService
    {
        Task<string> UploadImage(IFormFile file);
        Task<string> uploadImageForUser(int userId, IFormFile file);
        Task<string> uploadImageForOrder(int orderId, IFormFile file);
        Task<string> uploadImageForProduct(int productId, IFormFile file);
    }
}
