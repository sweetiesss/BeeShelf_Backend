using BeeStore_Api.Controllers;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace BeeStore_Api_Test.Controllers
{
    public class InventoryControllerTests
    {
        private readonly Mock<IRoomService> _mockInventoryService;
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly RoomController _inventoryController;

        public InventoryControllerTests()
        {
            _mockInventoryService = new Mock<IRoomService>();
            _mockMemoryCache = new Mock<IMemoryCache>();
            _inventoryController = new RoomController(_mockInventoryService.Object, _mockMemoryCache.Object);
        }

        [Fact]
        public async Task GetInventoryList_ShouldReturnInventoryList()
        {
            _mockInventoryService.Setup(s => s.GetInventoryList(null, null, null, false, 0, 10))
                .ReturnsAsync(new Pagination<RoomListDTO>());

            var response = await _inventoryController.GetInventoryList(null, null, null, false, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.IsType<Pagination<RoomListDTO>>(response.Value);
        }

        [Fact]
        public async Task GetInventoryById_ShouldReturnInventory()
        {
            var inventory = new RoomLotListDTO { Id = 1 };
            _mockInventoryService.Setup(s => s.GetInventoryById(1)).ReturnsAsync(inventory);

            var response = await _inventoryController.GetInventoryById(1) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(inventory, response.Value);
        }

        [Fact]
        public async Task AddPartnerToInventory_ShouldReturnSuccessMessage()
        {
            string successMessage = "Partner added to inventory successfully.";
            _mockInventoryService.Setup(s => s.AddPartnerToInventory(1, 1)).ReturnsAsync(successMessage);

            var response = await _inventoryController.AddPartnerToInventory(1, 1) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(successMessage, response.Value);
        }

        [Fact]
        public async Task CreateInventory_ShouldReturnCreatedInventory()
        {
            var inventory = new RoomCreateDTO { Name = "Test Inventory" };

            _mockInventoryService.Setup(s => s.CreateInventory(inventory)).ReturnsAsync(ResponseMessage.Success);

            var response = await _inventoryController.CreateInventory(inventory) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task UpdateInventory_ShouldReturnUpdatedInventory()
        {
            var inventoryUpdate = new RoomUpdateDTO { Id = 1, Name = "Updated Inventory" };

            _mockInventoryService.Setup(s => s.UpdateInventory(inventoryUpdate)).ReturnsAsync(ResponseMessage.Success);

            var response = await _inventoryController.UpdateInventory(inventoryUpdate) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task DeleteInventory_ShouldReturnSuccessMessage()
        {
            _mockInventoryService.Setup(s => s.DeleteInventory(1)).ReturnsAsync(ResponseMessage.Success);

            var response = await _inventoryController.DeleteInventory(1) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }
    }
}
