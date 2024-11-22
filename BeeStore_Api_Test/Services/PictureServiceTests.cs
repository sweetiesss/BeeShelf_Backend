using Amazon.S3;
using Amazon.S3.Transfer;
using BeeStore_Repository;
using BeeStore_Repository.Interfaces;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq.Expressions;

namespace BeeStore_Api_Test.Services
{
    public class PictureServiceTests
    {
        private readonly Mock<ITransferUtility> _mockTransferUtility;
        private readonly Mock<IAmazonS3> _mockS3Client;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Employee>> _mockEmployeeRepo;
        private readonly Mock<IGenericRepository<Order>> _mockOrderRepo;
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
        private readonly PictureService _pictureService;

        private const string BucketName = "test-bucket";
        private const string S3Url = "https://s3.amazonaws.com/test-bucket";

        public PictureServiceTests()
        {
            _mockS3Client = new Mock<IAmazonS3>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockEmployeeRepo = new Mock<IGenericRepository<Employee>>();
            _mockOrderRepo = new Mock<IGenericRepository<Order>>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();

            _mockTransferUtility = new Mock<ITransferUtility>();

            // Setup UnitOfWork to return the respective mock repositories
            _mockUnitOfWork.Setup(u => u.EmployeeRepo).Returns(_mockEmployeeRepo.Object);
            _mockUnitOfWork.Setup(u => u.OrderRepo).Returns(_mockOrderRepo.Object);
            _mockUnitOfWork.Setup(u => u.ProductRepo).Returns(_mockProductRepo.Object);

            // Initialize PictureService with mocks
            _pictureService = new PictureService(_mockS3Client.Object, BucketName, S3Url, _mockUnitOfWork.Object, _mockTransferUtility.Object);
        }

        private IFormFile CreateMockFormFile(string fileName, string contentType, byte[] content)
        {
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        [Fact]
        public async Task UploadImage_ShouldReturnImageUrl_WhenFileIsValid()
        {
            // Arrange
            var file = CreateMockFormFile("test.jpg", "image/jpeg", new byte[] { 1, 2, 3, 4 });


            // Setup
            _mockTransferUtility
                .Setup(tu => tu.UploadAsync(It.IsAny<TransferUtilityUploadRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _pictureService.UploadImage(file);

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith(S3Url, result);
            _mockTransferUtility.Verify(tu => tu.UploadAsync(It.IsAny<TransferUtilityUploadRequest>(), It.IsAny<CancellationToken>()), Times.Once);

        }



        [Fact]
        public async Task UploadImage_ShouldReturnNull_WhenFileIsNull()
        {
            // Act
            var result = await _pictureService.UploadImage(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UploadImage_ShouldThrowException_WhenS3UploadFails()
        {
            // Arrange
            var file = CreateMockFormFile("test.jpg", "image/jpeg", new byte[] { 1, 2, 3, 4 });

            // Setup
            _mockTransferUtility
                .Setup(tu => tu.UploadAsync(It.IsAny<TransferUtilityUploadRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("S3 upload failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _pictureService.UploadImage(file));
        }

        [Fact]
        public async Task UploadImageForUser_ShouldUpdateUserPictureLink()
        {
            // Arrange
            var file = CreateMockFormFile("test.jpg", "image/jpeg", new byte[] { 1, 2, 3, 4 });
            var user = new Employee { Id = 1, PictureLink = null };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                    .ReturnsAsync(user);

            // Act
            var result = await _pictureService.uploadImageForUser(user.Id, file);

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith(S3Url, result);
            _mockEmployeeRepo.Verify(repo => repo.Update(user), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UploadImageForOrder_ShouldUpdateOrderPictureLink()
        {
            // Arrange
            var file = CreateMockFormFile("test.jpg", "image/jpeg", new byte[] { 1, 2, 3, 4 });
            var order = new Order { Id = 1, PictureLink = null };

            _mockUnitOfWork.Setup(uow => uow.OrderRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                        .ReturnsAsync(order);

            // Act
            var result = await _pictureService.uploadImageForOrder(order.Id, file);

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith(S3Url, result);
            _mockOrderRepo.Verify(repo => repo.Update(order), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UploadImageForProduct_ShouldUpdateProductPictureLink()
        {
            // Arrange
            var file = CreateMockFormFile("test.jpg", "image/jpeg", new byte[] { 1, 2, 3, 4 });
            var product = new Product { Id = 1, PictureLink = null };

            _mockProductRepo
                .Setup(repo => repo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>,
                    IQueryable<Product>>>()))
                        .ReturnsAsync(product);

            // Act
            var result = await _pictureService.uploadImageForProduct(product.Id, file);

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith(S3Url, result);
            _mockProductRepo.Verify(repo => repo.Update(product), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
