using AutoMapper;
using BeeStore_Repository;
using BeeStore_Repository.DTO.Batch;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq.Expressions;

namespace BeeStore_Api_Test.Services
{
    public class BatchServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerManager> _mockLogger;
        private readonly BatchService _batchService;

        public BatchServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerManager>();
            _batchService = new BatchService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateBatch_Success()
        {
            // Arrange
            var batchCreateDto = new BatchCreateDTO
            {
                Name = "Test Batch",
                DeliveryZoneId = 1,
                Orders = new List<BatchOrdersCreate>
                {
                    new BatchOrdersCreate { Id = 1 }
                }
            };

            var order1 = new Order { Id = 1, BatchId = null, IsDeleted = false };

            var mappedBatch = new Batch
            {
                Id = 1,
                Name = "Test Batch",
                DeliveryZoneId = 1,
                Status = Constants.Status.Pending,
                IsDeleted = false,
                Orders = new List<Order>()
                {
                    order1
                }
            };

            foreach (var o in batchCreateDto.Orders)
            {
                _mockUnitOfWork
                    .Setup(u => u.OrderRepo.AnyAsync(u => u.Id.Equals(o.Id) && u.BatchId != null))
                    .ReturnsAsync(false);

                _mockUnitOfWork
                    .Setup(u => u.OrderRepo.AnyAsync(u => u.Id.Equals(o.Id) && u.IsDeleted == false))
                    .ReturnsAsync(true);
            }

            _mockMapper.Setup(mapper => mapper.Map<Batch>(batchCreateDto))
                .Returns(mappedBatch);

            _mockUnitOfWork
                .Setup(u => u.OrderRepo.SingleOrDefaultAsync(
                    It.Is<Expression<Func<Order, bool>>>(expr => expr.Compile()(order1)),
                    null))
                .ReturnsAsync(order1);

            _mockUnitOfWork.Setup(uow => uow.BatchRepo.AddAsync(It.IsAny<Batch>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(uow => uow.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _batchService.CreateBatch(batchCreateDto);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(u => u.BatchRepo.AddAsync(It.IsAny<Batch>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Exactly(batchCreateDto.Orders.Count + 1)); // Once for batch creation, once per order update
        }


        [Fact]
        public async Task CreateBatch_Throws_OrderAlreadyAssigned()
        {
            // Arrange
            var batchCreateDto = new BatchCreateDTO
            {
                Name = "Test Batch",
                DeliveryZoneId = 1,
                Orders = new List<BatchOrdersCreate>
            {
                new BatchOrdersCreate { Id = 1 }
            }
            };

            _mockUnitOfWork
                .Setup(u => u.OrderRepo.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(true); // Mock conflicting order

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _batchService.CreateBatch(batchCreateDto));
            Assert.Equal(ResponseMessage.OrderBatchError, exception.Message);
            _mockUnitOfWork.Verify(u => u.BatchRepo.AddAsync(It.IsAny<Batch>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateBatch_Throws_OrderNotFound()
        {
            // Arrange
            var batchCreateDto = new BatchCreateDTO
            {
                Name = "Test Batch",
                DeliveryZoneId = 1,
                Orders = new List<BatchOrdersCreate>
            {
                new BatchOrdersCreate { Id = 99 }
            }
            };

            _mockUnitOfWork
                .Setup(u => u.OrderRepo.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(false); // Mock order not found

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _batchService.CreateBatch(batchCreateDto));
            Assert.Equal(ResponseMessage.OrderIdNotFound, exception.Message);
            _mockUnitOfWork.Verify(u => u.BatchRepo.AddAsync(It.IsAny<Batch>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateBatch_Success()
        {
            // Arrange
            int batchId = 1;
            var batchUpdateDto = new BatchCreateDTO
            {
                Name = "Updated Batch",
                DeliveryZoneId = 2,
                Orders = new List<BatchOrdersCreate>
                {
                    new BatchOrdersCreate { Id = 2 }
                }
            };

            var existingBatch = new Batch
            {
                Id = batchId,
                Name = "Test Batch",
                Status = Constants.Status.Pending,
                Orders = new List<Order>
                {
                    new Order { Id = 1, BatchId = batchId }
                }
            };

            var newOrder = new Order { Id = 2, BatchId = null, IsDeleted = false };

            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync(existingBatch);

            _mockUnitOfWork
                .Setup(u => u.OrderRepo.AnyAsync(It.Is<Expression<Func<Order, bool>>>(expr => expr.Compile()(newOrder))))
                .ReturnsAsync(false);

            _mockUnitOfWork
                .Setup(u => u.OrderRepo.AnyAsync(It.Is<Expression<Func<Order, bool>>>(expr => expr.Compile()(new Order { Id = 2, IsDeleted = false }))))
                .ReturnsAsync(true);

            _mockMapper.Setup(mapper => mapper.Map<Batch>(batchUpdateDto))
                .Returns(new Batch { Id = batchId, Name = "Updated Batch", DeliveryZoneId = 2 });

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _batchService.UpdateBatch(batchId, batchUpdateDto);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Exactly(existingBatch.Orders.Count + 1)); // Clearing old orders + saving new orders
            _mockUnitOfWork.Verify(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBatch_Throws_BatchNotFound()
        {
            // Arrange
            int batchId = 99;
            var batchUpdateDto = new BatchCreateDTO
            {
                Name = "Updated Batch",
                DeliveryZoneId = 2,
                Orders = new List<BatchOrdersCreate> { new BatchOrdersCreate { Id = 1 } }
            };

            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync((Batch)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _batchService.UpdateBatch(batchId, batchUpdateDto));
            Assert.Equal(ResponseMessage.BatchIdNotFound, exception.Message);
        }

        [Fact]
        public async Task UpdateBatch_Throws_BatchStatusNotPending()
        {
            // Arrange
            int batchId = 1;
            var batchUpdateDto = new BatchCreateDTO
            {
                Name = "Updated Batch",
                DeliveryZoneId = 2,
                Orders = new List<BatchOrdersCreate> { new BatchOrdersCreate { Id = 1 } }
            };

            var existingBatch = new Batch
            {
                Id = batchId,
                Name = "Test Batch",
                Status = "Completed"
            };

            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync(existingBatch);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _batchService.UpdateBatch(batchId, batchUpdateDto));
            Assert.Equal(ResponseMessage.BatchStatusNotPending, exception.Message);
        }

        [Fact]
        public async Task UpdateBatch_Throws_OrderNotFound()
        {
            // Arrange
            int batchId = 1;
            var batchUpdateDto = new BatchCreateDTO
            {
                Name = "Updated Batch",
                DeliveryZoneId = 2,
                Orders = new List<BatchOrdersCreate> { new BatchOrdersCreate { Id = 99 } }
            };

            var existingBatch = new Batch
            {
                Id = batchId,
                Name = "Test Batch",
                Status = Constants.Status.Pending
            };

            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync(existingBatch);

            _mockUnitOfWork
                .Setup(u => u.OrderRepo.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _batchService.UpdateBatch(batchId, batchUpdateDto));
            Assert.Equal(ResponseMessage.OrderIdNotFound, exception.Message);
        }

        [Fact]
        public async Task UpdateBatch_Throws_OrderAlreadyAssigned()
        {
            // Arrange
            int batchId = 1;
            var batchUpdateDto = new BatchCreateDTO
            {
                Name = "Updated Batch",
                DeliveryZoneId = 2,
                Orders = new List<BatchOrdersCreate> { new BatchOrdersCreate { Id = 1 } }
            };

            var existingBatch = new Batch
            {
                Id = batchId,
                Name = "Test Batch",
                Status = Constants.Status.Pending
            };

            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync(existingBatch);

            _mockUnitOfWork
                .Setup(u => u.OrderRepo.AnyAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _batchService.UpdateBatch(batchId, batchUpdateDto));
            Assert.Equal(ResponseMessage.OrderBatchError, exception.Message);
        }
        [Fact]
        public async Task DeleteBatch_Success()
        {
            // Arrange
            int batchId = 1;
            var batchToDelete = new Batch
            {
                Id = batchId,
                Status = Constants.Status.Pending,
                Orders = new List<Order>
        {
            new Order { Id = 1, BatchId = batchId },
            new Order { Id = 2, BatchId = batchId }
        }
            };

            // Mock the BatchRepo to return the batch when finding by ID
            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync(batchToDelete);

            // Mock the SaveAsync method to return a completed task
            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _batchService.DeleteBatch(batchId);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(u => u.BatchRepo.SoftDelete(It.Is<Batch>(b => b.Id == batchId)), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Exactly(batchToDelete.Orders.Count + 1)); // Once for clearing BatchId and once for batch soft delete
        }

        [Fact]
        public async Task DeleteBatch_Throws_BatchNotFound()
        {
            // Arrange
            int batchId = 1;

            // Mock the BatchRepo to return null when the batch is not found
            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync((Batch)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _batchService.DeleteBatch(batchId));
            Assert.Equal(ResponseMessage.BatchIdNotFound, exception.Message);
            _mockUnitOfWork.Verify(u => u.BatchRepo.SoftDelete(It.IsAny<Batch>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteBatch_Throws_BatchStatusNotPending()
        {
            // Arrange
            int batchId = 1;
            var batchToDelete = new Batch
            {
                Id = batchId,
                Status = Constants.Status.Completed, // Not pending
                Orders = new List<Order>
        {
            new Order { Id = 1, BatchId = batchId }
        }
            };

            // Mock the BatchRepo to return the batch
            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync(batchToDelete);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _batchService.DeleteBatch(batchId));
            Assert.Equal(ResponseMessage.BatchStatusNotPending, exception.Message);
            _mockUnitOfWork.Verify(u => u.BatchRepo.SoftDelete(It.IsAny<Batch>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }
        [Fact]
        public async Task AssignBatch_Success()
        {
            // Arrange
            int batchId = 1;
            int shipperId = 2;

            var batch = new Batch
            {
                Id = batchId,
                BatchDeliveries = new List<BatchDelivery>()
            };

            var shipper = new Employee
            {
                Id = shipperId,
                RoleId = 4 // Role 4 indicates "Shipper"
            };

            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync(batch);

            _mockUnitOfWork
                .Setup(u => u.EmployeeRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Employee, bool>>>(), It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync(shipper);

            _mockUnitOfWork.Setup(uow => uow.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _batchService.AssignBatch(batchId, shipperId);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Single(batch.BatchDeliveries); // Ensure a BatchDelivery was added
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task AssignBatch_Throws_BatchNotFound()
        {
            // Arrange
            int batchId = 1;
            int shipperId = 2;

            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync((Batch)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _batchService.AssignBatch(batchId, shipperId));
            Assert.Equal(ResponseMessage.BatchIdNotFound, exception.Message);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task AssignBatch_Throws_ShipperNotFound()
        {
            // Arrange
            int batchId = 1;
            int shipperId = 2;

            var batch = new Batch
            {
                Id = batchId,
                BatchDeliveries = new List<BatchDelivery>()
            };

            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync(batch);

            _mockUnitOfWork
                .Setup(u => u.EmployeeRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Employee, bool>>>(), It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync((Employee)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _batchService.AssignBatch(batchId, shipperId));
            Assert.Equal(ResponseMessage.UserIdNotFound, exception.Message);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task AssignBatch_Throws_UserRoleNotShipper()
        {
            // Arrange
            int batchId = 1;
            int shipperId = 2;

            var batch = new Batch
            {
                Id = batchId,
                BatchDeliveries = new List<BatchDelivery>()
            };

            var nonShipper = new Employee
            {
                Id = shipperId,
                RoleId = 3 // Role not "Shipper"
            };

            _mockUnitOfWork
                .Setup(u => u.BatchRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Batch, bool>>>(), It.IsAny<Func<IQueryable<Batch>, IQueryable<Batch>>>()))
                .ReturnsAsync(batch);

            _mockUnitOfWork
                .Setup(u => u.EmployeeRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Employee, bool>>>(), It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync(nonShipper);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _batchService.AssignBatch(batchId, shipperId));
            Assert.Equal(ResponseMessage.UserRoleNotShipperError, exception.Message);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }
        [Fact]
        public async Task GetBatchList_Success()
        {
            // Arrange
            string search = "Test";
            BatchFilter? filterBy = BatchFilter.Status;
            string filterQuery = "Pending";
            int pageIndex = 1;
            int pageSize = 10;

            var batches = new List<Batch>
            {
                new Batch { Id = 1, Name = "Batch 1", Status = "Pending", DeliveryZoneId = 1 },
                new Batch { Id = 2, Name = "Batch 2", Status = "Pending", DeliveryZoneId = 1 }
            };

            var batchDTOs = new List<BatchListDTO>
            {
                new BatchListDTO { Id = 1, Name = "Batch 1", Status = "Pending", DeliveryZoneId = 1 },
                new BatchListDTO { Id = 2, Name = "Batch 2", Status = "Pending", DeliveryZoneId = 1 }
            };

            _mockUnitOfWork
                .Setup(u => u.BatchRepo.GetListAsync(
                    It.IsAny<Expression<Func<Batch, bool>>>(),
                    null,
                    null,
                    false,
                    search,
                    It.IsAny<Expression<Func<Batch, string>>[]>()))
                .ReturnsAsync(batches);

            _mockMapper.Setup(m => m.Map<List<BatchListDTO>>(batches))
                .Returns(batchDTOs);

            // Act
            var result = await _batchService.GetBatchList(search, filterBy, filterQuery, pageIndex, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pageIndex, result.PageIndex);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(batchDTOs.Count, result.TotalItemsCount);
        }

        [Fact]
        public async Task GetBatchList_Throws_BadRequest_WhenFilterMismatch()
        {
            // Arrange
            string search = "Test";
            BatchFilter? filterBy = BatchFilter.Status;
            string filterQuery = null; // FilterQuery is null while FilterBy is not
            int pageIndex = 1;
            int pageSize = 10;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadHttpRequestException>(() =>
                _batchService.GetBatchList(search, filterBy, filterQuery, pageIndex, pageSize));

            Assert.Equal(ResponseMessage.BadRequest, exception.Message);
        }

        [Fact]
        public async Task GetBatchList_Returns_EmptyPagination_WhenNoResults()
        {
            // Arrange
            string search = "NonExisting";
            BatchFilter? filterBy = null;
            string filterQuery = null;
            int pageIndex = 1;
            int pageSize = 10;

            _mockUnitOfWork
                .Setup(u => u.BatchRepo.GetListAsync(
                    null,
                    null,
                    null,
                    false,
                    search,
                    It.IsAny<Expression<Func<Batch, string>>[]>()))
                .ReturnsAsync(new List<Batch>());

            _mockMapper.Setup(m => m.Map<List<BatchListDTO>>(It.IsAny<List<Batch>>()))
                .Returns(new List<BatchListDTO>());

            // Act
            var result = await _batchService.GetBatchList(search, filterBy, filterQuery, pageIndex, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalItemsCount);
        }

    }
}
