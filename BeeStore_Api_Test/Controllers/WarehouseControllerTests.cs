using BeeStore_Api.Controllers;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Api_Test.Controllers
{
    public class WarehouseControllerTests
    {
        private readonly Mock<IWarehouseService> _mockWarehouseService;
        private readonly Mock<IWarehouseShipperService> _mockWarehouseShipperService;
        private readonly Mock<IWarehouseStaffService> _mockWarehouseStaffService;
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly WarehouseController _warehouseController;

        public WarehouseControllerTests()
        {
            _mockWarehouseService = new Mock<IWarehouseService>();
            _mockWarehouseShipperService = new Mock<IWarehouseShipperService>();
            _mockWarehouseStaffService = new Mock<IWarehouseStaffService>();
            _mockMemoryCache = new Mock<IMemoryCache>();
            _warehouseController = new WarehouseController(_mockWarehouseService.Object,
                                                           _mockWarehouseShipperService.Object,
                                                           _mockWarehouseStaffService.Object,
                                                           _mockMemoryCache.Object);
        }

        [Fact]
        public async Task GetWarehouseList_ShouldReturnWarehouseList()
        {
            var warehouses = new Pagination<WarehouseListDTO>();
            _mockWarehouseService
                .Setup(s => s.GetWarehouseList(null, null, null, null, false, 0, 10))
                .ReturnsAsync(warehouses);

            var response = await _warehouseController.GetWarehouseList(null, null, null, null, false, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(warehouses, response.Value);
        }

        [Fact]
        public async Task GetWarehouse_ShouldReturnWarehouseById()
        {
            var warehouse = new WarehouseDeliveryZoneDTO();
            var warehouseId = 1;
            _mockWarehouseService.Setup(s => s.GetWarehouseById(warehouseId)).ReturnsAsync(warehouse);

            var response = await _warehouseController.GetWarehouse(warehouseId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(warehouse, response.Value);
        }

        [Fact]
        public async Task GetWarehouseByUserId_ShouldReturnWarehouseListForUser()
        {
            var userId = 1;
            var warehouses = new List<WarehouseListInventoryDTO>();
            _mockWarehouseService.Setup(s => s.GetWarehouseByUserId(userId)).ReturnsAsync(warehouses);

            var response = await _warehouseController.GetWarehouseByUserId(userId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(warehouses, response.Value);
        }

        [Fact]
        public async Task GetWarehouseShipperList_ShouldReturnWarehouseShippers()
        {
            var shippers = new Pagination<WarehouseShipperListDTO>();
            _mockWarehouseShipperService
                .Setup(s => s.GetWarehouseShipperList(null, null, null, 0, 10))
                .ReturnsAsync(shippers);

            var response = await _warehouseController.GetWarehouseShipperList(null, null, null, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(shippers, response.Value);
        }

        [Fact]
        public async Task AddWarehouseShipper_ShouldReturnAddedShippers()
        {
            var newShippers = new List<WarehouseShipperCreateDTO> ();
            _mockWarehouseShipperService
                .Setup(s => s.AddShipperToWarehouse(newShippers))
                .ReturnsAsync(ResponseMessage.Success);

            var response = await _warehouseController.AddWarehouseShipper(newShippers) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task GetWarehouseStaffList_ShouldReturnWarehouseStaffs()
        {
            var staff = new Pagination<WarehouseStaffListDTO>();
            _mockWarehouseStaffService
                .Setup(s => s.GetWarehouseStaffList(null, null, null, 0, 10))
                .ReturnsAsync(staff);

            var response = await _warehouseController.GetWarehouseStaffList(null, null, null, 0, 10) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(staff, response.Value);
        }

        [Fact]
        public async Task AddWarehouseStaff_ShouldReturnAddedStaff()
        {
            var newStaff = new List<WarehouseStaffCreateDTO>();
            _mockWarehouseStaffService
                .Setup(s => s.AddStaffToWarehouse(newStaff))
                .ReturnsAsync(ResponseMessage.Success);

            var response = await _warehouseController.AddWarehouseStaff(newStaff) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task CreateWarehouse_ShouldReturnCreatedWarehouse()
        {
            var newWarehouse = new WarehouseCreateDTO();
            _mockWarehouseService
                .Setup(s => s.CreateWarehouse(newWarehouse))
                .ReturnsAsync(ResponseMessage.Success);

            var response = await _warehouseController.CreateWarehouse(newWarehouse) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task UpdateWarehouse_ShouldReturnUpdatedWarehouse()
        {
            var warehouseId = 1;
            var updatedWarehouse = new WarehouseCreateDTO();
            _mockWarehouseService
                .Setup(s => s.UpdateWarehouse(warehouseId, updatedWarehouse))
                .ReturnsAsync(ResponseMessage.Success);

            var response = await _warehouseController.UpdateWarehouse(warehouseId, updatedWarehouse) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task DeleteWarehouse_ShouldReturnDeleteMessage()
        {
            var warehouseId = 1;
            _mockWarehouseService
                .Setup(s => s.DeleteWarehouse(warehouseId))
                .ReturnsAsync(ResponseMessage.Success);

            var response = await _warehouseController.DeleteWarehouse(warehouseId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

    }
}
