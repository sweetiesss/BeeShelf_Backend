using BeeStore_Api.Controllers;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BeeStore_Api_Test.Controllers
{
    public class LotControllerTests
    {
        private readonly Mock<ILotService> _mockLotService;
        private readonly LotController _lotController;

        public LotControllerTests()
        {
            _mockLotService = new Mock<ILotService>();
            _lotController = new LotController(_mockLotService.Object);
        }

        [Fact]
        public async Task GetAllLots_AdminOrManagerRole_ShouldReturnAllLots()
        {
            _mockLotService.Setup(s => s.GetAllLots(null, null, null, null, false, 0, 10))
                .ReturnsAsync(new Pagination<LotListDTO>());

            var response = await _lotController.GetAllLots(null, null, null, null, false, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.IsType<Pagination<LotListDTO>>(response.Value);
        }

        [Fact]
        public async Task GetAllLots_ByUserId_ShouldReturnUserLots()
        {
            var userId = 1;
            _mockLotService.Setup(s => s.GetLotsByUserId(userId, null, null, null, null, false, 0, 10))
                .ReturnsAsync(new Pagination<LotListDTO>());

            var response = await _lotController.GetAllLots(userId, null, null, null, null, false, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.IsType<Pagination<LotListDTO>>(response.Value);
        }

        [Fact]
        public async Task GetLotById_ShouldReturnLot()
        {
            var lot = new LotListDTO { Id = 1 };
            _mockLotService.Setup(s => s.GetLotById(1)).ReturnsAsync(lot);

            var response = await _lotController.GetLotById(1) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(lot, response.Value);
        }

        [Fact]
        public async Task DeleteLot_ShouldReturnSuccessMessage()
        {
            _mockLotService.Setup(s => s.DeleteLot(1)).ReturnsAsync(ResponseMessage.Success);

            var response = await _lotController.DeleteLot(1) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }
    }
}
