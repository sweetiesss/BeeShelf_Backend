using Amazon.S3;
using Amazon.S3.Transfer;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;

namespace BeeStore_Repository.Services
{
    public class PictureService : IPictureService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _s3Url;
        private readonly IUnitOfWork _unitOfWork;




        public PictureService(IAmazonS3 s3Client, string bucketName, string s3Url, IUnitOfWork unitOfWork)
        {
            _s3Client = s3Client;
            _bucketName = bucketName;
            _s3Url = s3Url;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            try
            {
                string key = $"Image/{Guid.NewGuid()}";
                string imageUrl = $"{_s3Url}/{key}";

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    var fileTransferUtility = new TransferUtility(_s3Client);
                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = _bucketName,
                        Key = key,
                        InputStream = memoryStream,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                }


                return imageUrl;
            }
            catch (Exception ex)
            {
                throw new Exception(ResponseMessage.PictureUploadImageException);
            }
        }

        public async Task<string> uploadImageForUser(int userId, IFormFile file)
        {
            string imageUrl = await UploadImage(file);

            var user = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id == userId);
            user.PictureLink = imageUrl;

            _unitOfWork.EmployeeRepo.Update(user);
            await _unitOfWork.SaveAsync();

            return imageUrl;
        }

        public async Task<string> uploadImageForOrder(int orderId, IFormFile file)
        {
            string imageUrl = await UploadImage(file);

            //var picture = await getPictureByLink(imageUrl);

            var order = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == orderId);
            order.PictureLink = imageUrl;
            _unitOfWork.OrderRepo.Update(order);
            await _unitOfWork.SaveAsync();

            return imageUrl;
        }

        public async Task<string> uploadImageForProduct(int productId, IFormFile file)
        {
            string imageUrl = await UploadImage(file);

            var product = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id == productId);
            product.PictureLink = imageUrl;

            _unitOfWork.ProductRepo.Update(product);
            await _unitOfWork.SaveAsync();

            return imageUrl;
        }

    }


}
