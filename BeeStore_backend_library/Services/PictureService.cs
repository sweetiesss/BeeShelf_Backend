using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;

namespace BeeStore_Repository.Services
{
    public class PictureService : IPictureService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _s3Url;

        public PictureService(IAmazonS3 s3Client, string bucketName, string s3Url)
        {
            _s3Client = s3Client;
            _bucketName = bucketName;
            _s3Url = s3Url;
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {

                return null;
            }

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
    }

    public interface IPictureService
    {
        Task<string> UploadImage(IFormFile file);
    }
}
