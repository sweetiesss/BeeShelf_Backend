//using BeeStore_Repository.DTO.PaymentDTOs;
//using BeeStore_Repository.Logger;
//using BeeStore_Repository.Models;
//using BeeStore_Repository.Services;
//using BeeStore_Repository.Utils;
//using BeeStore_Repository;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AutoMapper;
//using Microsoft.Extensions.Configuration;
//using System.Linq.Expressions;
//using Azure.Security.KeyVault.Secrets;
//using BeeStore_Repository.Enums;
//using Newtonsoft.Json.Serialization;
//using Newtonsoft.Json;
//using System.Net;
//using Moq.Protected;
//using Azure;
//using Azure.Identity;

//namespace BeeStore_Api_Test.Services
//{
//    public class PaymentServiceTests
//    {
//        private readonly PaymentService _paymentService;
//        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
//        private readonly Mock<IMapper> _mockMapper;
//        private readonly Mock<ILoggerManager> _mockLogger;
//        private readonly Mock<SecretClient> _mockSecretClient;
//        private readonly Mock<IConfiguration> _mockConfiguration;

//        public PaymentServiceTests()
//        {
//            _mockUnitOfWork = new Mock<IUnitOfWork>();
//            _mockMapper = new Mock<IMapper>();
//            _mockLogger = new Mock<ILoggerManager>();
//            _mockSecretClient = new Mock<SecretClient>();
//            _mockConfiguration = new Mock<IConfiguration>();

//            _mockConfiguration.Setup(c => c["KeyVault:KeyVaultURL"]).Returns("https://mock-keyvault-url");

//            _mockSecretClient = new Mock<SecretClient>(MockBehavior.Strict, new Uri("https://fake-key-vault.vault.azure.net/"), new EnvironmentCredential());

//            _paymentService = new PaymentService(
//                _mockUnitOfWork.Object,
//                _mockMapper.Object,
//                _mockLogger.Object,
//                _mockConfiguration.Object);
//        }

//        private Response<KeyVaultSecret> CreateMockKeyVaultResponse(string name, string value)
//        {
//            var secret = new KeyVaultSecret(name, value);
//            var mockResponse = new Mock<Response<KeyVaultSecret>>();
//            mockResponse.Setup(r => r.Value).Returns(secret);
//            return mockResponse.Object;

//            /*
//             * 
//            _mockSecretClient
//                .Setup(client => client.GetSecret("BeeStore-Payment-ClientId", null, It.IsAny<CancellationToken>()))
//                .Returns(CreateMockKeyVaultResponse("BeeStore-Payment-ClientId", "mock-client-id"));
//            _mockSecretClient
//                .Setup(client => client.GetSecret("BeeStore-Payment-ApiKey", null, It.IsAny<CancellationToken>()))
//                .Returns(CreateMockKeyVaultResponse("BeeStore-Payment-ApiKey", "mock-api-key"));
//            _mockSecretClient
//                .Setup(client => client.GetSecret("BeeStore-Payment-CheckSumKey", null, It.IsAny<CancellationToken>()))
//                .Returns(CreateMockKeyVaultResponse("BeeStore-Payment-CheckSumKey", "mock-checksum-key"));

//            */
//        }

//        [Fact]
//        public async Task ConfirmPayment_ShouldReturnSuccess_WhenPaymentIsConfirmed()
//        {
//            // Arrange
//            var mockRequest = new ConfirmPaymentDTO
//            {
//                OrderCode = "ORD123",
//                Status = Constants.PaymentStatus.Paid
//            };

//            var mockTransaction = new Transaction
//            {
//                Code = "ORD123",
//                Status = Constants.PaymentStatus.Pending,
//                Amount = 100,
//                OcopPartnerId = 1
//            };

//            var mockWallet = new Wallet
//            {
//                OcopPartnerId = 1,
//                TotalAmount = 500
//            };

//            _mockUnitOfWork.Setup(u => u.TransactionRepo.SingleOrDefaultAsync(
//                It.IsAny<Expression<Func<Transaction, bool>>>(),
//                It.IsAny<Func<IQueryable<Transaction>, IQueryable<Transaction>>>()))
//                .ReturnsAsync(mockTransaction);

//            _mockUnitOfWork.Setup(u => u.WalletRepo.SingleOrDefaultAsync(
//                It.IsAny<Expression<Func<Wallet, bool>>>(),
//                null))
//                .ReturnsAsync(mockWallet);

//            // Act
//            var result = await _paymentService.ConfirmPayment(mockRequest);

//            // Assert
//            Assert.Equal(ResponseMessage.Success, result);
//            Assert.Equal(600, mockWallet.TotalAmount); // 500 + 100
//            Assert.Equal(Constants.PaymentStatus.Paid, mockTransaction.Status);

//            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
//        }

//        [Fact]
//        public async Task ConfirmPayment_ShouldThrowKeyNotFoundException_WhenTransactionDoesNotExist()
//        {
//            // Arrange
//            var mockRequest = new ConfirmPaymentDTO
//            {
//                OrderCode = "ORD123",
//                Status = Constants.PaymentStatus.Paid
//            };

//            _mockUnitOfWork.Setup(u => u.TransactionRepo.SingleOrDefaultAsync(
//                It.IsAny<Expression<Func<Transaction, bool>>>(),
//                null))
//                .ReturnsAsync((Transaction)null);

//            // Act & Assert
//            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _paymentService.ConfirmPayment(mockRequest));
//            Assert.Equal(ResponseMessage.TransactionNotFound, exception.Message);

//            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
//        }

//        [Fact]
//        public async Task ConfirmPayment_ShouldThrowApplicationException_WhenTransactionAlreadyProcessed()
//        {
//            // Arrange
//            var mockRequest = new ConfirmPaymentDTO
//            {
//                OrderCode = "ORD123",
//                Status = Constants.PaymentStatus.Paid
//            };

//            var mockTransaction = new Transaction
//            {
//                Code = "ORD123",
//                Status = Constants.PaymentStatus.Paid, // Already processed
//                Amount = 100
//            };

//            _mockUnitOfWork.Setup(u => u.TransactionRepo.SingleOrDefaultAsync(
//                It.IsAny<Expression<Func<Transaction, bool>>>(),
//                null))
//                .ReturnsAsync(mockTransaction);

//            // Act & Assert
//            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _paymentService.ConfirmPayment(mockRequest));
//            Assert.Equal(ResponseMessage.TransactionAlreadyProcessed, exception.Message);

//            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
//        }

//        [Fact]
//        public async Task ConfirmPayment_ShouldNotUpdateWallet_WhenStatusIsNotPaid()
//        {
//            // Arrange
//            var mockRequest = new ConfirmPaymentDTO
//            {
//                OrderCode = "ORD123",
//                Status = Constants.PaymentStatus.Cancelled
//            };

//            var mockTransaction = new Transaction
//            {
//                Code = "ORD123",
//                Status = Constants.PaymentStatus.Pending,
//                Amount = 100,
//                OcopPartnerId = 1
//            };

//            _mockUnitOfWork.Setup(u => u.TransactionRepo.SingleOrDefaultAsync(
//                It.IsAny<Expression<Func<Transaction, bool>>>(),
//                null))
//                .ReturnsAsync(mockTransaction);

//            // Act
//            var result = await _paymentService.ConfirmPayment(mockRequest);

//            // Assert
//            Assert.Equal(ResponseMessage.Success, result);
//            Assert.Equal(Constants.PaymentStatus.Cancelled, mockTransaction.Status);

//            _mockUnitOfWork.Verify(u => u.WalletRepo.SingleOrDefaultAsync(
//                It.IsAny<Expression<Func<Wallet, bool>>>(),
//                null), Times.Never);
//            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
//        }

//        [Fact]
//        public async Task CreateQrCode_ShouldThrowException_WhenUserNotFound()
//        {
//            // Arrange
//            var mockRequest = new PaymentRequestDTO
//            {
//                BuyerEmail = "nonexistent@example.com",
//                CancelUrl = "https://example.com/cancel",
//                Description = "Test Payment",
//                ReturnUrl = "https://example.com/return"
//            };

//            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
//                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
//                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
//                .ReturnsAsync((OcopPartner)null);

//            var coinPack = CoinPackValue.Ten_Thousand_VND;

//            // Act & Assert
//            await Assert.ThrowsAsync<KeyNotFoundException>(() => _paymentService.CreateQrCode(coinPack, null, mockRequest));

//            _mockUnitOfWork.Verify(u => u.TransactionRepo.AddAsync(It.IsAny<Transaction>()), Times.Never);
//            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
//        }

//        [Fact]
//        public async Task CreateQrCode_ShouldThrowException_WhenPaymentApiFails()
//        {
//            // Arrange
//            var mockRequest = new PaymentRequestDTO
//            {
//                BuyerEmail = "testuser@example.com",
//                CancelUrl = "https://example.com/cancel",
//                Description = "Test Payment",
//                ReturnUrl = "https://example.com/return"
//            };

//            var mockUser = new OcopPartner { Id = 1, Email = "testuser@example.com", FirstName = "John", LastName = "Doe", Phone = "1234567890" };

//            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
//                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
//                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
//                .ReturnsAsync(mockUser);

//            _mockUnitOfWork.Setup(u => u.TransactionRepo.AnyAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
//                .ReturnsAsync(false);

//            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
//            var mockHttpClient = new Mock<HttpMessageHandler>();
//            mockHttpClient.Protected()
//                .Setup(handler => handler.Send(It.IsAny<HttpRequestMessage>()))
//                .Returns(new HttpResponseMessage
//                {
//                    StatusCode = HttpStatusCode.BadRequest,
//                    Content = new StringContent("Bad Request", Encoding.UTF8, "application/json")
//                });

//            var coinPack = CoinPackValue.Ten_Thousand_VND;

//            // Act & Assert
//            await Assert.ThrowsAsync<Exception>(() => _paymentService.CreateQrCode(coinPack, null, mockRequest));

//            _mockUnitOfWork.Verify(u => u.TransactionRepo.AddAsync(It.IsAny<Transaction>()), Times.Never);
//            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
//        }
//    }
//}
