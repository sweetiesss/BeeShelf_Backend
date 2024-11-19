using BeeStore_Api.Controllers;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.DTO.WalletDTO;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BeeStore_Api_Test.Controllers
{
    public class PartnerControllerTests
    {
        private readonly Mock<IPartnerService> _mockPartnerService;
        private readonly Mock<IWalletService> _mockWalletService;
        private readonly PartnerController _partnerController;

        public PartnerControllerTests()
        {
            _mockPartnerService = new Mock<IPartnerService>();
            _mockWalletService = new Mock<IWalletService>();
            _partnerController = new PartnerController(_mockPartnerService.Object, _mockWalletService.Object);
        }

        [Fact]
        public async Task GetPartnerList_ShouldReturnPartnerList()
        {
            // Arrange
            var partnerList = new Pagination<PartnerListDTO>();

            _mockPartnerService.Setup(service => service.GetAllPartners(null, null, false, 0, 10))
                .ReturnsAsync(partnerList);

            // Act
            var response = await _partnerController.GetPartnerList(null, null, false, 0, 10) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(partnerList, response.Value);
        }

        [Fact]
        public async Task GetPartner_WithValidEmail_ShouldReturnPartner()
        {
            // Arrange
            var email = "partner1@example.com";
            var partner = new PartnerListDTO { Id = 1, Email = email, FirstName = "Partner 1" };

            _mockPartnerService.Setup(service => service.GetPartner(email))
                .ReturnsAsync(partner);

            // Act
            var response = await _partnerController.GetPartner(email) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(partner, response.Value);
        }

        [Fact]
        public async Task GetWalletByUserId_WithValidId_ShouldReturnWallet()
        {
            // Arrange
            var userEmail = "partner1@example.com";
            var userId = 1;
            var wallet = new WalletDTO { partner_email = userEmail, TotalAmount = 500 };

            _mockWalletService.Setup(service => service.GetWalletByUserId(userId))
                .ReturnsAsync(wallet);

            // Act
            var response = await _partnerController.GetWalletByUserId(userId) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(wallet, response.Value);
        }

        [Fact]
        public async Task UpdatePartner_ShouldReturnUpdatedPartner()
        {
            // Arrange
            var updateRequest = new OCOPPartnerUpdateRequest
            {
                Email = "partner1@example.com",
                FirstName = "Updated Partner Name",
                Phone = "123-456-7890"
            };

            _mockPartnerService.Setup(service => service.UpdatePartner(updateRequest))
                .ReturnsAsync(ResponseMessage.Success);

            // Act
            var response = await _partnerController.UpdatePartner(updateRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task DeletePartner_ShouldReturnSuccessMessage()
        {
            // Arrange
            var partnerId = 1;
            var deleteMessage = "Partner deleted successfully.";

            _mockPartnerService.Setup(service => service.DeletePartner(partnerId))
                .ReturnsAsync(deleteMessage);

            // Act
            var response = await _partnerController.DeletePartner(partnerId) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(deleteMessage, response.Value);
        }
    }
}
