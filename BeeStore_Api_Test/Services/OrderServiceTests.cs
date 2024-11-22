using AutoMapper;
using BeeStore_Repository;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq.Expressions;

namespace BeeStore_Api_Test.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerManager> _mockLogger;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerManager>();
            _orderService = new OrderService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnPaginatedList_WhenDataExists()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order
                {
                Id = 1,
                OcopPartnerId = 1,
                Status = "Pending",
                CreateDate = DateTime.Now.AddDays(-5),
                ReceiverPhone = "1234567890",
                ReceiverAddress = "Address 1",
                Distance = 15.5m,
                TotalPrice = 100.0m,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        Id = 1,
                        LotId = 101,
                        ProductPrice = 50,
                        ProductAmount = 2
                    }
                },
                OrderFees = new List<OrderFee>
                {
                    new OrderFee { DeliveryFee = 10.0m, StorageFee = 5.0m, AdditionalFee = 2.0m }
                }
            },
                new Order
                {
                    Id = 2,
                    OcopPartnerId = 2,
                    Status = "Completed",
                    CreateDate = DateTime.Now.AddDays(-10),
                    ReceiverPhone = "9876543210",
                    ReceiverAddress = "Address 2",
                    Distance = 10.0m,
                    TotalPrice = 200.0m,
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail
                        {
                            Id = 2,
                            LotId = 102,
                            ProductPrice = 100,
                            ProductAmount = 1
                        }
                    },
                    OrderFees = new List<OrderFee>
                    {
                        new OrderFee { DeliveryFee = 15.0m, StorageFee = 10.0m, AdditionalFee = 5.0m }
                    }
                }
            };

            var orderDTOs = orders.Select(o => new OrderListDTO
            {
                Id = o.Id,
                partner_email = o.OcopPartner?.Email,
                Status = o.Status,
                CreateDate = o.CreateDate,
                ReceiverPhone = o.ReceiverPhone,
                ReceiverAddress = o.ReceiverAddress,
                Distance = o.Distance,
                TotalPrice = o.TotalPrice,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
                {
                    Id = od.Id,
                    LotId = od.LotId,
                    ProductPrice = od.ProductPrice,
                    ProductAmount = od.ProductAmount
                }).ToList(),
                OrderFees = o.OrderFees.Select(of => new OrderFeeDTO
                {
                    DeliveryFee = of.DeliveryFee,
                    StorageFee = of.StorageFee,
                    AdditionalFee = of.AdditionalFee
                }).ToList()
            }).ToList();

            var paginationResult = new Pagination<OrderListDTO>
            {
                PageIndex = 0,
                PageSize = 10,
                TotalItemsCount = orderDTOs.Count,
                Items = orderDTOs
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.GetListAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<Expression<Func<Order, string>>[]>()))
                .ReturnsAsync(orders);

            _mockMapper.Setup(mapper => mapper.Map<List<OrderListDTO>>(orders))
                .Returns(orderDTOs);

            // Act
            var result = await _orderService.GetOrderList(
                orderStatus: null,
                sortCriteria: null,
                descending: false,
                pageIndex: 0,
                pageSize: 10);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginationResult.TotalItemsCount, result.TotalItemsCount);
            Assert.Equal(paginationResult.Items.Count, result.Items.Count);
            Assert.Equal(paginationResult.Items.First().Id, result.Items.First().Id);
            Assert.Equal(paginationResult.Items.First().OrderDetails.First().ProductName, result.Items.First().OrderDetails.First().ProductName);
        }
        [Fact]
        public async Task GetWarehouseSentOrderList_ShouldReturnPaginatedList_WhenDataExists()
        {
            // Arrange
            int warehouseId = 1;
            OrderStatus? orderStatus = OrderStatus.Pending;
            OrderSortBy? sortCriteria = OrderSortBy.CreateDate;
            bool descending = false;
            int pageIndex = 0;
            int pageSize = 10;

            var orders = new List<Order>
    {
        new Order
        {
            Id = 1,
            OcopPartnerId = 1,
            Status = "Pending",
            CreateDate = DateTime.Now.AddDays(-5),
            ReceiverPhone = "1234567890",
            ReceiverAddress = "Address 1",
            Distance = 15.5m,
            TotalPrice = 100.0m,
            OrderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    Id = 1,
                    LotId = 101,
                    ProductPrice = 50,
                    ProductAmount = 2
                }
            },
            OrderFees = new List<OrderFee>
            {
                new OrderFee { DeliveryFee = 10.0m, StorageFee = 5.0m, AdditionalFee = 2.0m }
            }
        },
        new Order
        {
            Id = 2,
            OcopPartnerId = 2,
            Status = "Pending",
            CreateDate = DateTime.Now.AddDays(-10),
            ReceiverPhone = "9876543210",
            ReceiverAddress = "Address 2",
            Distance = 10.0m,
            TotalPrice = 200.0m,
            OrderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    Id = 2,
                    LotId = 102,
                    ProductPrice = 100,
                    ProductAmount = 1
                }
            },
            OrderFees = new List<OrderFee>
            {
                new OrderFee { DeliveryFee = 15.0m, StorageFee = 10.0m, AdditionalFee = 5.0m }
            }
        }
    };

            var orderDTOs = orders.Select(o => new OrderListDTO
            {
                Id = o.Id,
                partner_email = o.OcopPartner?.Email,
                Status = o.Status,
                CreateDate = o.CreateDate,
                ReceiverPhone = o.ReceiverPhone,
                ReceiverAddress = o.ReceiverAddress,
                Distance = o.Distance,
                TotalPrice = o.TotalPrice,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
                {
                    Id = od.Id,
                    LotId = od.LotId,
                    ProductPrice = od.ProductPrice,
                    ProductAmount = od.ProductAmount
                }).ToList(),
                OrderFees = o.OrderFees.Select(of => new OrderFeeDTO
                {
                    DeliveryFee = of.DeliveryFee,
                    StorageFee = of.StorageFee,
                    AdditionalFee = of.AdditionalFee
                }).ToList()
            }).ToList();

            var paginationResult = new Pagination<OrderListDTO>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = orderDTOs.Count,
                Items = orderDTOs
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.GetListAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<Expression<Func<Order, string>>[]>()))
                .ReturnsAsync(orders);

            _mockMapper.Setup(mapper => mapper.Map<List<OrderListDTO>>(orders))
                .Returns(orderDTOs);

            // Act
            var result = await _orderService.GetWarehouseSentOrderList(
                warehouseId: warehouseId,
                orderStatus: orderStatus,
                sortCriteria: sortCriteria,
                descending: descending,
                pageIndex: pageIndex,
                pageSize: pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginationResult.TotalItemsCount, result.TotalItemsCount);
            Assert.Equal(paginationResult.Items.Count, result.Items.Count);
            Assert.Equal(paginationResult.Items.First().Id, result.Items.First().Id);
            Assert.Equal(paginationResult.Items.First().OrderDetails.First().ProductName, result.Items.First().OrderDetails.First().ProductName);
        }
        [Fact]
        public async Task GetOrderList_ShouldReturnPaginatedList_WhenDataExists()
        {
            // Arrange
            int userId = 1;
            OrderStatus? orderStatus = OrderStatus.Pending;
            OrderSortBy? sortCriteria = OrderSortBy.CreateDate;
            bool descending = false;
            int pageIndex = 0;
            int pageSize = 10;

            var orders = new List<Order>
    {
        new Order
        {
            Id = 1,
            OcopPartnerId = 1,
            Status = "Pending",
            CreateDate = DateTime.Now.AddDays(-5),
            ReceiverPhone = "1234567890",
            ReceiverAddress = "Address 1",
            Distance = 15.5m,
            TotalPrice = 100.0m
        },
        new Order
        {
            Id = 2,
            OcopPartnerId = 2,
            Status = "Pending",
            CreateDate = DateTime.Now.AddDays(-10),
            ReceiverPhone = "9876543210",
            ReceiverAddress = "Address 2",
            Distance = 10.0m,
            TotalPrice = 200.0m
        }
    };

            var orderDTOs = orders.Select(o => new OrderListDTO
            {
                Id = o.Id,
                partner_email = o.OcopPartner?.Email,
                Status = o.Status,
                CreateDate = o.CreateDate,
                ReceiverPhone = o.ReceiverPhone,
                ReceiverAddress = o.ReceiverAddress,
                Distance = o.Distance,
                TotalPrice = o.TotalPrice
            }).ToList();

            var paginationResult = new Pagination<OrderListDTO>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = orderDTOs.Count,
                Items = orderDTOs
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.GetListAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<Expression<Func<Order, string>>[]>()))
                .ReturnsAsync(orders);

            _mockMapper.Setup(mapper => mapper.Map<List<OrderListDTO>>(orders))
                .Returns(orderDTOs);

            // Act
            var result = await _orderService.GetOrderList(
                userId: userId,
                orderStatus: orderStatus,
                sortCriteria: sortCriteria,
                descending: descending,
                pageIndex: pageIndex,
                pageSize: pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginationResult.TotalItemsCount, result.TotalItemsCount);
            Assert.Equal(paginationResult.Items.Count, result.Items.Count);
            Assert.Equal(paginationResult.Items.First().Id, result.Items.First().Id);
        }
        [Fact]
        public async Task GetDeliverOrderList_ShouldReturnPaginatedList_WhenDataExists()
        {
            // Arrange
            int shipperId = 2;
            OrderStatus? orderStatus = OrderStatus.Delivered;
            OrderSortBy? sortCriteria = OrderSortBy.TotalPrice;
            bool descending = true;
            int pageIndex = 1;
            int pageSize = 5;

            var orders = new List<Order>
            {
                new Order
                {
                    Id = 3,
                    OcopPartnerId = 3,
                    Status = "Delivered",
                    CreateDate = DateTime.Now.AddDays(-15),
                    ReceiverPhone = "5432167890",
                    ReceiverAddress = "Address 3",
                    Distance = 25.5m,
                    TotalPrice = 150.0m
                },
                new Order
                {
                    Id = 4,
                    OcopPartnerId = 4,
                    Status = "Delivered",
                    CreateDate = DateTime.Now.AddDays(-20),
                    ReceiverPhone = "0987654321",
                    ReceiverAddress = "Address 4",
                    Distance = 30.0m,
                    TotalPrice = 300.0m
                }
            };

            var orderDTOs = orders.Select(o => new OrderListDTO
            {
                Id = o.Id,
                partner_email = o.OcopPartner?.Email,
                Status = o.Status,
                CreateDate = o.CreateDate,
                ReceiverPhone = o.ReceiverPhone,
                ReceiverAddress = o.ReceiverAddress,
                Distance = o.Distance,
                TotalPrice = o.TotalPrice
            }).ToList();

            var paginationResult = new Pagination<OrderListDTO>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = orderDTOs.Count,
                Items = orderDTOs
            };

            // Setup
            _mockUnitOfWork.Setup(u => u.OrderRepo.GetListAsync(
                    It.IsAny<Expression<Func<Order, bool>>>(),
                    It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<Expression<Func<Order, string>>[]>()))
                .ReturnsAsync(orders);

            _mockMapper.Setup(mapper => mapper.Map<List<OrderListDTO>>(orders))
                .Returns(orderDTOs);

            // Act
            var result = await _orderService.GetDeliverOrderList(
                shipperId: shipperId,
                orderStatus: orderStatus,
                sortCriteria: sortCriteria,
                descending: descending,
                pageIndex: pageIndex,
                pageSize: pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginationResult.TotalItemsCount, result.TotalItemsCount);
        }
        [Fact]
        public async Task CreateOrder_ShouldThrowKeyNotFoundException_WhenOcopPartnerNotFound()
        {
            // Arrange
            var orderRequest = new OrderCreateDTO
            {
                OcopPartnerId = 1,
                Products = new List<ProductDetailDTO>
                {
                    new ProductDetailDTO { ProductId = 101, ProductAmount = 5 }
                }
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()))
                .ReturnsAsync(false); // Simulating that the OcopPartner is not found

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.CreateOrder(orderRequest));
        }

        [Fact]
        public async Task CreateOrder_ShouldThrowKeyNotFoundException_WhenProductNotFound()
        {
            // Arrange
            var orderRequest = new OrderCreateDTO
            {
                OcopPartnerId = 1,
                Products = new List<ProductDetailDTO>
                {
                    new ProductDetailDTO { ProductId = 101, ProductAmount = 5 }
                }
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()))
                .ReturnsAsync(true); // Simulating OcopPartner exists

            _mockUnitOfWork.Setup(u => u.ProductRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
                .ReturnsAsync((Product)null); // Simulating product not found

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.CreateOrder(orderRequest));
        }

        [Fact]
        public async Task CreateOrder_ShouldThrowApplicationException_WhenNotEnoughProduct()
        {
            // Arrange
            var orderRequest = new OrderCreateDTO
            {
                OcopPartnerId = 1,
                Products = new List<ProductDetailDTO>
                {
                    new ProductDetailDTO { ProductId = 101, ProductAmount = 5 } // Requesting 5 products
                }
            };

            var mockProduct = new Product { Id = 101, Price = 50 };

            var mockLot = new Lot
            {
                Id = 1,
                ProductId = 101,
                ProductAmount = 3, // Only 3 products available, less than the requested 5
                IsDeleted = false,
                Inventory = new Inventory { WarehouseId = 1 },
                Product = mockProduct
            };

            // Setup
            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(u => u.ProductRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
                .ReturnsAsync(mockProduct);

            _mockUnitOfWork.Setup(u => u.LotRepo.GetQueryable(It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync(new List<Lot> { mockLot });

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(async () => await _orderService.CreateOrder(orderRequest));
        }


        [Fact]
        public async Task CreateOrder_ShouldThrowKeyNotFoundException_WhenNoLotAvailable()
        {
            // Arrange
            var orderRequest = new OrderCreateDTO
            {
                OcopPartnerId = 1,
                Products = new List<ProductDetailDTO>
        {
            new ProductDetailDTO { ProductId = 101, ProductAmount = 5 }
        }
            };

            var mockProduct = new Product { Id = 101, Price = 50 };

            // Mocking the dependencies
            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()))
                .ReturnsAsync(true); // OcopPartner exists

            _mockUnitOfWork.Setup(u => u.ProductRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
                .ReturnsAsync(mockProduct); // Product exists

            _mockUnitOfWork.Setup(u => u.LotRepo.GetQueryable(It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync(new List<Lot>()); // Simulating no lots available (product not enough)

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.CreateOrder(orderRequest));
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnSuccess_WhenOrderIsCreated()
        {
            // Arrange
            var orderRequest = new OrderCreateDTO
            {
                OcopPartnerId = 1,
                Products = new List<ProductDetailDTO>
        {
            new ProductDetailDTO { ProductId = 101, ProductAmount = 5 }
        }
            };

            var mockProduct = new Product { Id = 101, Price = 50 };
            var mockLot = new Lot
            {
                Id = 1,
                ProductId = 101,
                ProductAmount = 10,
                IsDeleted = false,
                Inventory = new Inventory { WarehouseId = 1 },
                Product = mockProduct
            };

            // Mocking the dependencies
            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()))
                .ReturnsAsync(true); // OcopPartner exists

            _mockUnitOfWork.Setup(u => u.ProductRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
                .ReturnsAsync(mockProduct); // Product exists

            _mockUnitOfWork.Setup(u => u.LotRepo.GetQueryable(It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync(new List<Lot> { mockLot }); // Lot is available

            _mockUnitOfWork.Setup(u => u.OrderRepo.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask); // Simulate successful order save
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask); // Simulate save

            // Mocking the IMapper behavior to return a valid Order object when mapping from OrderCreateDTO
            var mockOrder = new Order
            {
                Id = 1, // Assume a new Order is created with Id = 1
                CreateDate = DateTime.Now,
                Status = Constants.Status.Draft,
                TotalPrice = 250 // 50 * 5 products
            };

            _mockMapper.Setup(m => m.Map<Order>(It.IsAny<OrderCreateDTO>())).Returns(mockOrder); // Mock the mapping from DTO to Order

            // Mocking OrderFeeRepo to simulate adding the OrderFee
            _mockUnitOfWork.Setup(u => u.OrderFeeRepo.AddAsync(It.IsAny<OrderFee>())).Returns(Task.CompletedTask); // Simulate adding the order fee
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask); // Simulate saving the OrderFee

            // Act
            var result = await _orderService.CreateOrder(orderRequest);

            // Assert
            Assert.Equal(ResponseMessage.Success, result); // Expecting a success message
            _mockUnitOfWork.Verify(u => u.OrderRepo.AddAsync(It.IsAny<Order>()), Times.Once); // Verifying the Order was added
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Exactly(2)); // Verifying SaveAsync was called twice (once for Order and once for OrderFee)
            _mockUnitOfWork.Verify(u => u.OrderFeeRepo.AddAsync(It.IsAny<OrderFee>()), Times.Once); // Verifying OrderFee was added
        }

        [Fact]
        public async Task DeleteOrder_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1; // ID of the order to delete

            // Mocking the repository to return null (order not found)
            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync((Order)null); // Order does not exist

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.DeleteOrder(orderId));
            Assert.Equal(ResponseMessage.OrderIdNotFound, exception.Message);
        }

        [Fact]
        public async Task DeleteOrder_ShouldThrowApplicationException_WhenOrderIsNotInDraftStatus()
        {
            // Arrange
            int orderId = 1; // ID of the order to delete
            var existingOrder = new Order
            {
                Id = orderId,
                Status = Constants.Status.Approved // Not in Draft status
            };

            // Mocking the repository to return an order that is not in Draft status
            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(existingOrder);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(async () => await _orderService.DeleteOrder(orderId));
            Assert.Equal(ResponseMessage.OrderProccessedError, exception.Message);
        }
        [Fact]
        public async Task DeleteOrder_ShouldReturnSuccess_WhenOrderIsDeleted()
        {
            // Arrange
            int orderId = 1; // ID of the order to delete
            var existingOrder = new Order
            {
                Id = orderId,
                Status = Constants.Status.Draft // In Draft status
            };

            // Mocking the repository to return an order in Draft status
            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(existingOrder);

            _mockUnitOfWork.Setup(u => u.OrderRepo.SoftDelete(It.IsAny<Order>())).Verifiable();
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _orderService.DeleteOrder(orderId);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(u => u.OrderRepo.SoftDelete(It.IsAny<Order>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
        [Fact]
        public async Task UpdateOrder_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;
            var orderUpdateRequest = new OrderUpdateDTO
            {
                OrderDetails = new List<OrderDetailUpdateDTO>
        {
            new OrderDetailUpdateDTO { LotId = 1, ProductAmount = 5 }
        }
            };

            // Mocking the repository to return null (order not found)
            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync((Order)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.UpdateOrder(orderId, orderUpdateRequest));
            Assert.Equal(ResponseMessage.OrderIdNotFound, exception.Message);
        }

        [Fact]
        public async Task UpdateOrder_ShouldThrowKeyNotFoundException_WhenPackageDoesNotExist()
        {
            // Arrange
            int orderId = 1;
            var orderUpdateRequest = new OrderUpdateDTO
            {
                OrderDetails = new List<OrderDetailUpdateDTO>
        {
            new OrderDetailUpdateDTO { LotId = 1, ProductAmount = 5 }
        }
            };

            var existingOrder = new Order
            {
                Id = orderId,
                Status = Constants.Status.Draft,
                OrderDetails = new List<OrderDetail>()
            };

            // Mocking the repository to return the existing order
            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(existingOrder);

            // Mocking the repository to return null (package not found)
            _mockUnitOfWork.Setup(u => u.LotRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Lot, bool>>>(), It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync((Lot)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.UpdateOrder(orderId, orderUpdateRequest));
            Assert.Equal(ResponseMessage.PackageIdNotFound, exception.Message);
        }

        [Fact]
        public async Task UpdateOrder_ShouldThrowApplicationException_WhenOrderIsNotDraftOrShipping()
        {
            // Arrange
            int orderId = 1;
            var orderUpdateRequest = new OrderUpdateDTO
            {
                OrderDetails = new List<OrderDetailUpdateDTO>
            {
                new OrderDetailUpdateDTO { LotId = 1, ProductAmount = 5 }
            },
                ReceiverAddress = "New Address",
                ReceiverPhone = "1234567890"
            };

            var existingOrder = new Order
            {
                Id = orderId,
                Status = Constants.Status.Delivered, // Not in Draft or Shipping status
                OrderDetails = new List<OrderDetail>()
            };
            var mockLot = new Lot
            {
                Id = 1,
                ProductId = 101,
                ProductAmount = 10,
                Product = new Product { Id = 101, Price = 50 },
                Inventory = new Inventory { WarehouseId = 1 }
            };

            // Setup
            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(existingOrder);

            _mockUnitOfWork.Setup(u => u.LotRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Lot, bool>>>(), It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync(mockLot);
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(async () => await _orderService.UpdateOrder(orderId, orderUpdateRequest));
            Assert.Equal(ResponseMessage.OrderProccessedError, exception.Message);
        }

        [Fact]
        public async Task UpdateOrder_ShouldUpdateOrderDetails_WhenOrderStatusIsDraft()
        {
            // Arrange
            int orderId = 1;
            var orderUpdateRequest = new OrderUpdateDTO
            {
                OrderDetails = new List<OrderDetailUpdateDTO>
        {
            new OrderDetailUpdateDTO { LotId = 1, ProductAmount = 5 }
        },
                ReceiverAddress = "New Address",
                ReceiverPhone = "1234567890"
            };

            var existingOrder = new Order
            {
                Id = orderId,
                Status = Constants.Status.Draft, // In Draft status
                OrderDetails = new List<OrderDetail>(),
                OrderFees = new List<OrderFee>()
            };

            var lot = new Lot
            {
                Id = 1,
                ProductId = 101,
                ProductAmount = 10,
                Product = new Product { Id = 101, Price = 50 },
                Inventory = new Inventory { WarehouseId = 1 }
            };

            // Mocking the repository to return the existing order and lot
            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(existingOrder);

            _mockUnitOfWork.Setup(u => u.LotRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Lot, bool>>>(), It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync(lot);

            _mockMapper.Setup(m => m.Map<List<OrderDetail>>(It.IsAny<OrderUpdateDTO>()))
                .Returns(new List<OrderDetail> { new OrderDetail() });

            _mockUnitOfWork.Setup(u => u.OrderRepo.Update(It.IsAny<Order>())).Verifiable();
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _orderService.UpdateOrder(orderId, orderUpdateRequest);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(u => u.OrderRepo.Update(It.IsAny<Order>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateOrder_ShouldUpdateOrderFees_WhenOrderStatusIsShipping()
        {
            // Arrange
            int orderId = 1;
            var orderUpdateRequest = new OrderUpdateDTO
            {
                ReceiverAddress = "Updated Address",
                ReceiverPhone = "9876543210"
            };

            var existingOrder = new Order
            {
                Id = orderId,
                Status = Constants.Status.Shipping, // In Shipping status
                OrderFees = new List<OrderFee>
                {
                    new OrderFee { AdditionalFee = 0, DeliveryFee = 0, StorageFee = 0 }
                }
            };

            var mockLot = new Lot
            {
                Id = 1,
                ProductId = 101,
                ProductAmount = 10,
                Product = new Product { Id = 101, Price = 50 },
                Inventory = new Inventory { WarehouseId = 1 }
            };

            // Mocking the repository to return the existing order
            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(existingOrder);

            _mockUnitOfWork.Setup(u => u.LotRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Lot, bool>>>(), It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync(mockLot);
            _mockUnitOfWork.Setup(u => u.OrderRepo.Update(It.IsAny<Order>())).Verifiable();
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _orderService.UpdateOrder(orderId, orderUpdateRequest);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Equal(1, existingOrder.OrderFees.ElementAt(0).DeliveryFee);
            Assert.Equal(1, existingOrder.OrderFees.ElementAt(0).StorageFee);
            Assert.Equal(1, existingOrder.OrderFees.ElementAt(0).AdditionalFee);
            _mockUnitOfWork.Verify(u => u.OrderRepo.Update(It.IsAny<Order>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }
        [Fact]
        public async Task SendOrder_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync((Order)null); // Simulating the case where the order does not exist

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.SendOrder(orderId));
            Assert.Equal(ResponseMessage.OrderIdNotFound, exception.Message);
        }

        [Fact]
        public async Task SendOrder_ShouldThrowApplicationException_WhenOrderStatusIsNotDraft()
        {
            // Arrange
            int orderId = 1;
            var existingOrder = new Order
            {
                Id = orderId,
                Status = Constants.Status.Shipping // Simulating a non-Draft status
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(existingOrder); // Mocking the order as not Draft

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(async () => await _orderService.SendOrder(orderId));
            Assert.Equal(ResponseMessage.OrderSentError, exception.Message);
        }

        [Fact]
        public async Task SendOrder_ShouldReturnSuccess_WhenOrderIsSentSuccessfully()
        {
            // Arrange
            int orderId = 1;
            var existingOrder = new Order
            {
                Id = orderId,
                Status = Constants.Status.Draft // Simulating a Draft status
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(existingOrder); // Mocking the order to be in Draft status

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask); // Mock SaveAsync

            // Act
            var result = await _orderService.SendOrder(orderId);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Equal(Constants.Status.Pending, existingOrder.Status); // Assert that the status is updated to Pending
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once); // Ensure SaveAsync was called
        }
        [Fact]
        public async Task CancelOrder_ShouldReturnSuccess_WhenOrderIsCanceled()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Status = Constants.Status.Shipping // Simulate an order with a status that can be canceled
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(order); // Order exists with a cancellable status

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask); // Simulate saving the updated order

            // Act
            var result = await _orderService.CancelOrder(orderId);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Equal(Constants.Status.Canceled, order.Status);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelOrder_ShouldThrowApplicationException_WhenOrderCannotBeCanceled()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Status = Constants.Status.Draft // Simulate an order with a status that cannot be canceled
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(order); // Order exists but with a non-cancellable status

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(async () => await _orderService.CancelOrder(orderId));
        }

        [Fact]
        public async Task CancelOrder_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;
            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync((Order)null); // Order does not exist

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.CancelOrder(orderId));
        }
        [Fact]
        public async Task UpdateOrderStatus_ShouldReturnSuccess_WhenStatusIsUpdatedToProcessing()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Status = Constants.Status.Pending,
                OrderDetails = new List<OrderDetail> { new OrderDetail { LotId = 1, ProductAmount = 5 } }
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(order); // Order exists with Pending status

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask); // Simulate saving the updated order

            // Act
            var result = await _orderService.UpdateOrderStatus(orderId, OrderStatus.Processing);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Equal(Constants.Status.Processing, order.Status); // Verify status is updated to Processing
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once); // Verify SaveAsync is called
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldReturnSuccess_WhenStatusIsUpdatedToShipping()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Status = Constants.Status.Processing,
                OrderDetails = new List<OrderDetail> { new OrderDetail { LotId = 1, ProductAmount = 5 } }
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(order); // Order exists with Processing status

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask); // Simulate saving the updated order

            // Act
            var result = await _orderService.UpdateOrderStatus(orderId, OrderStatus.Shipping);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Equal(Constants.Status.Shipping, order.Status); // Verify status is updated to Shipping
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once); // Verify SaveAsync is called
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldReturnSuccess_WhenStatusIsUpdatedToCanceled()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Status = Constants.Status.Processing,
                OrderDetails = new List<OrderDetail> { new OrderDetail { LotId = 1, ProductAmount = 5 } }
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(order); // Order exists with Processing status

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask); // Simulate saving the updated order

            // Act
            var result = await _orderService.UpdateOrderStatus(orderId, OrderStatus.Canceled);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Equal(Constants.Status.Canceled, order.Status); // Verify status is updated to Canceled
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once); // Verify SaveAsync is called
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldThrowApplicationException_WhenStatusCannotBeUpdatedToProcessing()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Status = Constants.Status.Shipping // Shipping status cannot be updated to Processing
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(order); // Order exists with Shipping status

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(async () => await _orderService.UpdateOrderStatus(orderId, OrderStatus.Processing));
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldThrowApplicationException_WhenStatusCannotBeUpdatedToShipping()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Status = Constants.Status.Pending // Pending status cannot be updated to Shipping
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(order); // Order exists with Pending status

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(async () => await _orderService.UpdateOrderStatus(orderId, OrderStatus.Shipping));
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldThrowApplicationException_WhenStatusCannotBeUpdatedToCanceled()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Status = Constants.Status.Delivered // Delivered status cannot be updated to Canceled
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(order); // Order exists with Delivered status

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(async () => await _orderService.UpdateOrderStatus(orderId, OrderStatus.Canceled));
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldThrowKeyNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;
            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync((Order)null); // Order does not exist

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _orderService.UpdateOrderStatus(orderId, OrderStatus.Processing));
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldThrowBadHttpRequestException_WhenOrderStatusIsInvalid()
        {
            // Arrange
            int orderId = 1;
            var order = new Order
            {
                Id = orderId,
                Status = Constants.Status.Pending
            };

            _mockUnitOfWork.Setup(u => u.OrderRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(order); // Order exists with Pending status

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(async () => await _orderService.UpdateOrderStatus(orderId, (OrderStatus)999)); // Invalid status
        }

    }
}
