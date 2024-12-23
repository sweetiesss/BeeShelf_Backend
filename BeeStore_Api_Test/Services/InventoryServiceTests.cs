using AutoMapper;
using BeeStore_Repository;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services;
using BeeStore_Repository.Utils;
using Moq;
using System.Linq.Expressions;

namespace BeeStore_Api_Test.Services
{
    public class InventoryServiceTests
    {
        private readonly RoomService _inventoryService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerManager> _mockLogger;

        public InventoryServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerManager>();
            _inventoryService = new RoomService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddPartnerToInventory_Success()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 10;

            var inventory = new Inventory { Id = inventoryId, OcopPartnerId = null };
            var user = new Employee { Id = userId, RoleId = 4 };

            _mockUnitOfWork.Setup(u => u.InventoryRepo.GetByIdAsync(inventoryId)).ReturnsAsync(inventory);
            _mockUnitOfWork.Setup(u => u.EmployeeRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                    .ReturnsAsync(user);

            // Act
            var result = await _inventoryService.AddPartnerToInventory(inventoryId, userId);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Equal(userId, inventory.OcopPartnerId);
            _mockUnitOfWork.Verify(u => u.InventoryRepo.Update(inventory), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task AddPartnerToInventory_Throws_KeyNotFoundException_WhenInventoryNotFound()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 10;

            _mockUnitOfWork.Setup(u => u.InventoryRepo.GetByIdAsync(inventoryId)).ReturnsAsync((Inventory)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _inventoryService.AddPartnerToInventory(inventoryId, userId));
            Assert.Equal(ResponseMessage.InventoryIdNotFound, exception.Message);
        }

        [Fact]
        public async Task AddPartnerToInventory_Throws_DuplicateException_WhenInventoryAlreadyOccupied()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 10;

            var inventory = new Inventory { Id = inventoryId, OcopPartnerId = 5 };

            _mockUnitOfWork.Setup(u => u.InventoryRepo.GetByIdAsync(inventoryId)).ReturnsAsync(inventory);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DuplicateException>(() =>
                _inventoryService.AddPartnerToInventory(inventoryId, userId));
            Assert.Equal(ResponseMessage.InventoryOccupied, exception.Message);
        }

        [Fact]
        public async Task AddPartnerToInventory_Throws_KeyNotFoundException_WhenUserNotFound()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 10;

            var inventory = new Inventory { Id = inventoryId, OcopPartnerId = null };

            _mockUnitOfWork.Setup(u => u.InventoryRepo.GetByIdAsync(inventoryId)).ReturnsAsync(inventory);
            _mockUnitOfWork.Setup(u => u.EmployeeRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                    .ReturnsAsync((Employee)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _inventoryService.AddPartnerToInventory(inventoryId, userId));
            Assert.Equal(ResponseMessage.UserIdNotFound, exception.Message);
        }

        [Fact]
        public async Task AddPartnerToInventory_Throws_KeyNotFoundException_WhenUserIsNotPartner()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 10;

            var inventory = new Inventory { Id = inventoryId, OcopPartnerId = null };
            var user = new Employee { Id = userId, RoleId = 3 }; // Not a partner

            _mockUnitOfWork.Setup(u => u.InventoryRepo.GetByIdAsync(inventoryId)).ReturnsAsync(inventory);
            _mockUnitOfWork.Setup(u => u.EmployeeRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                    .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _inventoryService.AddPartnerToInventory(inventoryId, userId));
            Assert.Equal(ResponseMessage.UserRoleNotPartnerError, exception.Message);
        }
        [Fact]
        public async Task CreateInventory_ShouldReturnSuccess_WhenValidRequest()
        {
            // Arrange
            var request = new RoomCreateDTO
            {
                Name = "New Inventory",
                MaxWeight = 100,
                WarehouseId = 1
            };

            var warehouse = new Warehouse { Id = request.WarehouseId };

            _mockUnitOfWork.Setup(u => u.WarehouseRepo.GetByIdAsync(request.WarehouseId))
                .ReturnsAsync(warehouse);

            var inventory = new Inventory
            {
                Name = request.Name,
                MaxWeight = request.MaxWeight,
                WarehouseId = request.WarehouseId
            };

            _mockMapper.Setup(m => m.Map<Inventory>(request))
                .Returns(inventory);

            _mockUnitOfWork.Setup(u => u.InventoryRepo.AddAsync(inventory))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(u => u.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _inventoryService.CreateInventory(request);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(u => u.WarehouseRepo.GetByIdAsync(request.WarehouseId), Times.Once);
            _mockMapper.Verify(m => m.Map<Inventory>(request), Times.Once);
            _mockUnitOfWork.Verify(u => u.InventoryRepo.AddAsync(It.IsAny<Inventory>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateInventory_ShouldThrowKeyNotFoundException_WhenWarehouseDoesNotExist()
        {
            // Arrange
            var request = new RoomCreateDTO
            {
                Name = "New Inventory",
                MaxWeight = 100,
                WarehouseId = 1
            };

            _mockUnitOfWork.Setup(u => u.WarehouseRepo.GetByIdAsync(request.WarehouseId))
                .ReturnsAsync((Warehouse)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _inventoryService.CreateInventory(request));

            Assert.Equal(ResponseMessage.WarehouseIdNotFound, exception.Message);
            _mockUnitOfWork.Verify(u => u.WarehouseRepo.GetByIdAsync(request.WarehouseId), Times.Once);
            _mockMapper.Verify(m => m.Map<Inventory>(It.IsAny<RoomCreateDTO>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.InventoryRepo.AddAsync(It.IsAny<Inventory>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }
        [Fact]
        public async Task DeleteInventory_ShouldReturnSuccess_WhenInventoryExists()
        {
            // Arrange
            int inventoryId = 1;

            var inventory = new Inventory { Id = inventoryId };

            _mockUnitOfWork.Setup(u => u.InventoryRepo.GetByIdAsync(inventoryId))
                .ReturnsAsync(inventory);

            _mockUnitOfWork.Setup(u => u.InventoryRepo.SoftDelete(inventory));

            _mockUnitOfWork.Setup(u => u.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _inventoryService.DeleteInventory(inventoryId);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(u => u.InventoryRepo.GetByIdAsync(inventoryId), Times.Once);
            _mockUnitOfWork.Verify(u => u.InventoryRepo.SoftDelete(It.IsAny<Inventory>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteInventory_ShouldThrowKeyNotFoundException_WhenInventoryDoesNotExist()
        {
            // Arrange
            int inventoryId = 1;

            _mockUnitOfWork.Setup(u => u.InventoryRepo.GetByIdAsync(inventoryId))
                .ReturnsAsync((Inventory)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _inventoryService.DeleteInventory(inventoryId));

            Assert.Equal(ResponseMessage.InventoryIdNotFound, exception.Message);
            _mockUnitOfWork.Verify(u => u.InventoryRepo.GetByIdAsync(inventoryId), Times.Once);
            _mockUnitOfWork.Verify(u => u.InventoryRepo.SoftDelete(It.IsAny<Inventory>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }
        [Fact]
        public async Task UpdateInventory_ShouldReturnSuccess_WhenInventoryExistsAndValidRequest()
        {
            // Arrange
            var updateRequest = new RoomUpdateDTO
            {
                Id = 1,
                Name = "Updated Inventory",
                Weight = 50,
                MaxWeight = 100
            };

            var existingInventory = new Inventory
            {
                Id = updateRequest.Id,
                MaxWeight = 80 // Existing max weight
            };

            _mockUnitOfWork.Setup(u => u.InventoryRepo.GetByIdAsync(updateRequest.Id))
                .ReturnsAsync(existingInventory);

            _mockUnitOfWork.Setup(u => u.InventoryRepo.Update(existingInventory));

            _mockUnitOfWork.Setup(u => u.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _inventoryService.UpdateInventory(updateRequest);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Equal(updateRequest.MaxWeight, existingInventory.MaxWeight); // Ensure max weight is updated
            _mockUnitOfWork.Verify(u => u.InventoryRepo.GetByIdAsync(updateRequest.Id), Times.Once);
            _mockUnitOfWork.Verify(u => u.InventoryRepo.Update(It.IsAny<Inventory>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateInventory_ShouldThrowKeyNotFoundException_WhenInventoryDoesNotExist()
        {
            // Arrange
            var updateRequest = new RoomUpdateDTO
            {
                Id = 1,
                Name = "Non-existent Inventory",
                MaxWeight = 100
            };

            _mockUnitOfWork.Setup(u => u.InventoryRepo.GetByIdAsync(updateRequest.Id))
                .ReturnsAsync((Inventory)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _inventoryService.UpdateInventory(updateRequest));

            Assert.Equal(ResponseMessage.InventoryIdNotFound, exception.Message);
            _mockUnitOfWork.Verify(u => u.InventoryRepo.GetByIdAsync(updateRequest.Id), Times.Once);
            _mockUnitOfWork.Verify(u => u.InventoryRepo.Update(It.IsAny<Inventory>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }
        [Fact]
        public async Task GetInventoryList_ShouldReturnPaginatedResult_WhenFilterAndSortAreValid()
        {
            // Arrange
            var filterBy = RoomFilter.WarehouseId;
            var filterQuery = "1";
            var sortCriteria = RoomSortBy.Name;
            var descending = false;
            var pageIndex = 0;
            var pageSize = 2;

            var inventoryList = new List<Inventory>
    {
        new Inventory { Id = 1, Name = "Inventory1", WarehouseId = 1 },
        new Inventory { Id = 2, Name = "Inventory2", WarehouseId = 1 }
    };

            var mappedList = inventoryList.Select(i => new RoomListDTO
            {
                Id = i.Id,
                Name = i.Name,
                WarehouseName = $"Warehouse{i.WarehouseId}"
            }).ToList();

            _mockUnitOfWork.Setup(u => u.InventoryRepo.GetListAsync(
                It.IsAny<Expression<Func<Inventory, bool>>>(),
                null,
                It.IsAny<string>(),
                descending,
                null,
                null))
                .ReturnsAsync(inventoryList);

            _mockMapper.Setup(m => m.Map<List<RoomListDTO>>(inventoryList))
                .Returns(mappedList);

            // Act
            var result = await _inventoryService.GetInventoryList(filterBy, filterQuery, sortCriteria, descending, pageIndex, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedList.Count, result.Items.Count);
            Assert.Equal(pageIndex, result.PageIndex);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(inventoryList.Count, result.TotalItemsCount);

            _mockUnitOfWork.Verify(u => u.InventoryRepo.GetListAsync(It.IsAny<Expression<Func<Inventory, bool>>>(),
                null, It.IsAny<string>(), descending, null, null), Times.Once);
            _mockMapper.Verify(m => m.Map<List<RoomListDTO>>(inventoryList), Times.Once);
        }

        [Fact]
        public async Task GetInventoryListByUserId_ShouldReturnPaginatedResult_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var filterBy = RoomFilter.WarehouseId;
            var filterQuery = "1";
            var sortCriteria = RoomSortBy.Name;
            var descending = false;
            var pageIndex = 0;
            var pageSize = 2;

            var inventoryList = new List<Inventory>
    {
        new Inventory { Id = 1, Name = "Inventory1", WarehouseId = 1, OcopPartnerId = userId },
        new Inventory { Id = 2, Name = "Inventory2", WarehouseId = 1, OcopPartnerId = userId }
    };

            var mappedList = inventoryList.Select(i => new RoomListDTO
            {
                Id = i.Id,
                Name = i.Name,
                WarehouseName = $"Warehouse{i.WarehouseId}"
            }).ToList();

            _mockUnitOfWork.Setup(u => u.InventoryRepo.GetListAsync(
                It.IsAny<Expression<Func<Inventory, bool>>>(),
                null,
                It.IsAny<string>(),
                descending,
                null,
                null))
                .ReturnsAsync(inventoryList);

            _mockMapper.Setup(m => m.Map<List<RoomListDTO>>(inventoryList))
                .Returns(mappedList);

            // Act
            var result = await _inventoryService.GetInventoryList(userId, filterBy, filterQuery, sortCriteria, descending, pageIndex, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedList.Count, result.Items.Count);
            Assert.Equal(pageIndex, result.PageIndex);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(inventoryList.Count, result.TotalItemsCount);

            _mockUnitOfWork.Verify(u => u.InventoryRepo.GetListAsync(It.IsAny<Expression<Func<Inventory, bool>>>(),
                null, It.IsAny<string>(), descending, null, null), Times.Once);
            _mockMapper.Verify(m => m.Map<List<RoomListDTO>>(inventoryList), Times.Once);
        }
        [Fact]
        public async Task BuyInventory_ShouldReturnSuccess_WhenConditionsAreMet()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 1;

            var user = new OcopPartner
            {
                Id = userId,
                Wallets = new List<Wallet>
            {
                new Wallet { OcopPartnerId = userId, TotalAmount = 50000 }
            }
            };

            var inventory = new Inventory
            {
                Id = inventoryId,
                OcopPartnerId = null
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.InventoryRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Inventory, bool>>>(),
                It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
                .ReturnsAsync(inventory);

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _inventoryService.BuyInventory(inventoryId, userId, 1);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Equal(userId, inventory.OcopPartnerId);
            Assert.Equal(20000, user.Wallets.First().TotalAmount);

            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task BuyInventory_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 1;

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync((OcopPartner)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _inventoryService.BuyInventory(inventoryId, userId, 1));
            Assert.Equal(ResponseMessage.UserIdNotFound, exception.Message);
        }

        [Fact]
        public async Task BuyInventory_ShouldThrowKeyNotFoundException_WhenInventoryDoesNotExist()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 1;

            var user = new OcopPartner
            {
                Id = userId,
                Wallets = new List<Wallet> { new Wallet { OcopPartnerId = userId, TotalAmount = 50000 } }
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.InventoryRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Inventory, bool>>>(),
                It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
                .ReturnsAsync((Inventory)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _inventoryService.BuyInventory(inventoryId, userId, 1));
            Assert.Equal(ResponseMessage.InventoryIdNotFound, exception.Message);
        }

        [Fact]
        public async Task BuyInventory_ShouldThrowApplicationException_WhenInventoryIsOccupied()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 1;

            var user = new OcopPartner
            {
                Id = userId,
                Wallets = new List<Wallet> { new Wallet { OcopPartnerId = userId, TotalAmount = 50000 } }
            };

            var inventory = new Inventory
            {
                Id = inventoryId,
                OcopPartnerId = 2 // Occupied by another user
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.InventoryRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Inventory, bool>>>(),
                It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
                .ReturnsAsync(inventory);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _inventoryService.BuyInventory(inventoryId, userId, 1));
            Assert.Equal(ResponseMessage.InventoryOccupied, exception.Message);
        }

        [Fact]
        public async Task BuyInventory_ShouldThrowApplicationException_WhenNotEnoughCredit()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 1;

            var user = new OcopPartner
            {
                Id = userId,
                Wallets = new List<Wallet> { new Wallet { OcopPartnerId = userId, TotalAmount = 20000 } }
            };

            var inventory = new Inventory
            {
                Id = inventoryId,
                OcopPartnerId = null
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.InventoryRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Inventory, bool>>>(),
                It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
                .ReturnsAsync(inventory);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _inventoryService.BuyInventory(inventoryId, userId, 1));
            Assert.Equal(ResponseMessage.NotEnoughCredit, exception.Message);
        }
        [Fact]
        public async Task ExtendInventory_ShouldReturnSuccess_WhenConditionsAreMet()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 1;
            var initialExpirationDate = DateTime.Now;

            var user = new OcopPartner
            {
                Id = userId,
                Wallets = new List<Wallet>
            {
                new Wallet { OcopPartnerId = userId, TotalAmount = 50000 }
            }
            };

            var inventory = new Inventory
            {
                Id = inventoryId,
                OcopPartnerId = userId,
                ExpirationDate = initialExpirationDate
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.InventoryRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Inventory, bool>>>(),
                It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
                .ReturnsAsync(inventory);

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _inventoryService.ExtendInventory(inventoryId, userId, 1);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Equal(initialExpirationDate.AddDays(30), inventory.ExpirationDate);
            Assert.Equal(20000, user.Wallets.First().TotalAmount);

            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task ExtendInventory_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 1;

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync((OcopPartner)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _inventoryService.ExtendInventory(inventoryId, userId, 1));
            Assert.Equal(ResponseMessage.UserIdNotFound, exception.Message);
        }

        [Fact]
        public async Task ExtendInventory_ShouldThrowKeyNotFoundException_WhenInventoryDoesNotExist()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 1;

            var user = new OcopPartner
            {
                Id = userId,
                Wallets = new List<Wallet> { new Wallet { OcopPartnerId = userId, TotalAmount = 50000 } }
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.InventoryRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Inventory, bool>>>(),
                It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
                .ReturnsAsync((Inventory)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _inventoryService.ExtendInventory(inventoryId, userId, 1));
            Assert.Equal(ResponseMessage.InventoryIdNotFound, exception.Message);
        }

        [Fact]
        public async Task ExtendInventory_ShouldThrowApplicationException_WhenInventoryPartnerNotMatch()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 1;

            var user = new OcopPartner
            {
                Id = userId,
                Wallets = new List<Wallet> { new Wallet { OcopPartnerId = userId, TotalAmount = 50000 } }
            };

            var inventory = new Inventory
            {
                Id = inventoryId,
                OcopPartnerId = 2 // Owned by another user
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.InventoryRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Inventory, bool>>>(),
                It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
                .ReturnsAsync(inventory);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _inventoryService.ExtendInventory(inventoryId, userId, 1));
            Assert.Equal(ResponseMessage.InventoryPartnerNotMatch, exception.Message);
        }

        [Fact]
        public async Task ExtendInventory_ShouldThrowApplicationException_WhenNotEnoughCredit()
        {
            // Arrange
            int inventoryId = 1;
            int userId = 1;

            var user = new OcopPartner
            {
                Id = userId,
                Wallets = new List<Wallet> { new Wallet { OcopPartnerId = userId, TotalAmount = 20000 } }
            };

            var inventory = new Inventory
            {
                Id = inventoryId,
                OcopPartnerId = userId,
                ExpirationDate = DateTime.Now
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(u => u.InventoryRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Inventory, bool>>>(),
                It.IsAny<Func<IQueryable<Inventory>, IQueryable<Inventory>>>()))
                .ReturnsAsync(inventory);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _inventoryService.ExtendInventory(inventoryId, userId, 1));
            Assert.Equal(ResponseMessage.NotEnoughCredit, exception.Message);
        }
    }
}
