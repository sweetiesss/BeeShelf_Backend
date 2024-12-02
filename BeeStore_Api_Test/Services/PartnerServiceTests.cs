using AutoMapper;
using BeeStore_Repository;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;

namespace BeeStore_Api_Test.Services
{
    public class PartnerServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerManager> _mockLogger;
        private readonly PartnerService _partnerService;

        public PartnerServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerManager>();

            _partnerService = new PartnerService(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }
        [Fact]
        public async Task GetAllPartners_ShouldReturnPaginatedList_WhenDataExists()
        {
            // Arrange
            var pageIndex = 0;
            var pageSize = 2;
            var search = "test";
            var sortBy = SortBy.CreateDate;
            var descending = false;

            var mockPartners = new List<OcopPartner>
            {
                new OcopPartner
                {
                    Id = 1,
                    Email = "partner1@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    BusinessName = "Business One",
                    Province = new Province { Code = "P001", SubDivisionName = "Province 1" },
                    Category = new Category { Type = "Category 1" },
                    OcopCategory = new OcopCategory { Type = "Ocop Category 1" },
                    Role = new Role { RoleName = "Partner Role" }
                },
                new OcopPartner
                {
                    Id = 2,
                    Email = "partner2@example.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    BusinessName = "Business Two",
                    Province = new Province { Code = "P002", SubDivisionName = "Province 2" },
                    Category = new Category { Type = "Category 2" },
                    OcopCategory = new OcopCategory { Type = "Ocop Category 2" },
                    Role = new Role { RoleName = "Another Role" }
                }
            };

            var mockPartnerDTOs = new List<PartnerListDTO>
            {
                new PartnerListDTO
                {
                    Id = 1,
                    Email = "partner1@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    BusinessName = "Business One",
                    ProvinceCode = "P001",
                    CategoryName = "Category 1",
                    OcopCategoryName = "Ocop Category 1",
                    RoleName = "Partner Role"
                },
                new PartnerListDTO
                {
                    Id = 2,
                    Email = "partner2@example.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    BusinessName = "Business Two",
                    ProvinceCode = "P002",
                    CategoryName = "Category 2",
                    OcopCategoryName = "Ocop Category 2",
                    RoleName = "Another Role"
                }
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.GetListAsync(
                    It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                    It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<Expression<Func<OcopPartner, string>>[]>()
            )).ReturnsAsync(mockPartners);

            _mockMapper.Setup(m => m.Map<List<PartnerListDTO>>(mockPartners)).Returns(mockPartnerDTOs);

            // Act
            var result = await _partnerService.GetAllPartners(search, sortBy, descending, pageIndex, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(pageIndex, result.PageIndex);
            Assert.Equal(2, result.TotalItemsCount);
            Assert.False(result.Next);
            Assert.False(result.Previous);

            var firstPartner = result.Items.First();
            Assert.Equal(1, firstPartner.Id);
            Assert.Equal("partner1@example.com", firstPartner.Email);
            Assert.Equal("Business One", firstPartner.BusinessName);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.GetListAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>(),
                sortBy.ToString(),
                descending,
                search,
                new Expression<Func<OcopPartner, string>>[]
                {
                    p => p.Email,
                    p => p.FirstName,
                    p => p.LastName,
                    p => p.BusinessName
                }
            ), Times.Once);

            _mockMapper.Verify(m => m.Map<List<PartnerListDTO>>(mockPartners), Times.Once);
        }

        [Fact]
        public async Task GetPartner_ShouldReturnPartner_WhenEmailExists()
        {
            // Arrange
            var email = "partner1@example.com";

            var mockPartner = new OcopPartner
            {
                Id = 1,
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                BusinessName = "Business One",
                Province = new Province { Code = "P001", SubDivisionName = "Province 1" },
                Category = new Category { Type = "Category 1" },
                OcopCategory = new OcopCategory { Type = "Ocop Category 1" },
                Role = new Role { RoleName = "Partner Role" }
            };

            var mockPartnerDTO = new PartnerListDTO
            {
                Id = 1,
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                BusinessName = "Business One",
                ProvinceCode = "P001",
                CategoryName = "Category 1",
                OcopCategoryName = "Ocop Category 1",
                RoleName = "Partner Role"
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                    It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()
                )).ReturnsAsync(mockPartner);

            _mockMapper.Setup(m => m.Map<PartnerListDTO>(mockPartner)).Returns(mockPartnerDTO);

            // Act
            var result = await _partnerService.GetPartner(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockPartnerDTO.Id, result.Id);
            Assert.Equal(mockPartnerDTO.Email, result.Email);
            Assert.Equal(mockPartnerDTO.BusinessName, result.BusinessName);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                u => u.Email == email,
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()
            ), Times.Once);

            _mockMapper.Verify(m => m.Map<PartnerListDTO>(mockPartner), Times.Once);
        }

        [Fact]
        public async Task GetPartner_ShouldThrowKeyNotFoundException_WhenEmailDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                    It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()
                )).ReturnsAsync((OcopPartner)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _partnerService.GetPartner(email));
            Assert.Equal(ResponseMessage.UserEmailNotFound, exception.Message);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                u => u.Email == email,
                It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()
            ), Times.Once);

            _mockMapper.Verify(m => m.Map<PartnerListDTO>(It.IsAny<OcopPartner>()), Times.Never);
        }
        [Fact]
        public async Task UpdatePartner_ShouldReturnSuccess_WhenValidUpdateRequest()
        {
            // Arrange
            var mockEmail = "partner@example.com";
            var mockUpdateRequest = new OCOPPartnerUpdateRequest
            {
                Email = mockEmail,
                ConfirmPassword = "validpassword",
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Phone = "1234567890",
                BankName = "UpdatedBank",
                BankAccountNumber = "12345678",
                Setting = "NewSetting",
                PictureLink = "https://example.com/new-picture.jpg",
                ProvinceId = 1,
                CategoryId = 2,
                OcopCategoryId = 3,
                TaxIdentificationNumber = "123-45-6789",
                BusinessName = "UpdatedBusiness"
            };

            var existingPartner = new OcopPartner
            {
                Email = mockEmail,
                Password = BCrypt.Net.BCrypt.HashPassword("validpassword"),
                UpdateDate = DateTime.Now.AddDays(-31),
                Setting = "OldSetting",
                PictureLink = "https://example.com/old-picture.jpg"
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                    null
                )).ReturnsAsync(existingPartner);

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.Update(It.IsAny<OcopPartner>()));
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _partnerService.UpdatePartner(mockUpdateRequest);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            Assert.Equal(mockUpdateRequest.Setting, existingPartner.Setting);
            Assert.Equal(mockUpdateRequest.PictureLink, existingPartner.PictureLink);
            Assert.Equal(mockUpdateRequest.FirstName, existingPartner.FirstName);
            Assert.Equal(mockUpdateRequest.LastName, existingPartner.LastName);
            Assert.Equal(mockUpdateRequest.Phone, existingPartner.Phone);
            Assert.Equal(mockUpdateRequest.BankName, existingPartner.BankName);
            Assert.Equal(mockUpdateRequest.BankAccountNumber, existingPartner.BankAccountNumber);
            Assert.Equal(mockUpdateRequest.ProvinceId, existingPartner.ProvinceId);
            Assert.Equal(mockUpdateRequest.CategoryId, existingPartner.CategoryId);
            Assert.Equal(mockUpdateRequest.OcopCategoryId, existingPartner.OcopCategoryId);
            Assert.Equal(mockUpdateRequest.TaxIdentificationNumber, existingPartner.TaxIdentificationNumber);
            Assert.Equal(mockUpdateRequest.BusinessName, existingPartner.BusinessName);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.Update(existingPartner), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePartner_ShouldThrowApplicationException_WhenPasswordMismatch()
        {
            // Arrange
            var mockEmail = "partner@example.com";
            var mockUpdateRequest = new OCOPPartnerUpdateRequest
            {
                Email = mockEmail,
                ConfirmPassword = "invalidpassword"
            };

            var existingPartner = new OcopPartner
            {
                Email = mockEmail,
                Password = BCrypt.Net.BCrypt.HashPassword("validpassword"),
                UpdateDate = DateTime.Now.AddDays(-31)
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                    null
                )).ReturnsAsync(existingPartner);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _partnerService.UpdatePartner(mockUpdateRequest));
            Assert.Equal(ResponseMessage.UserPasswordError, exception.Message);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.Update(It.IsAny<OcopPartner>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdatePartner_ShouldThrowApplicationException_WhenUpdateIntervalTooShort()
        {
            // Arrange
            var mockEmail = "partner@example.com";
            var mockUpdateRequest = new OCOPPartnerUpdateRequest
            {
                Email = mockEmail,
                ConfirmPassword = "validpassword"
            };

            var existingPartner = new OcopPartner
            {
                Email = mockEmail,
                Password = BCrypt.Net.BCrypt.HashPassword("validpassword"),
                UpdateDate = DateTime.Now.AddDays(-10)
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                    null
                )).ReturnsAsync(existingPartner);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _partnerService.UpdatePartner(mockUpdateRequest));
            Assert.Equal(ResponseMessage.UpdatePartnerError, exception.Message);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.Update(It.IsAny<OcopPartner>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdatePartner_ShouldThrowKeyNotFoundException_WhenPartnerDoesNotExist()
        {
            // Arrange
            var mockEmail = "nonexistent@example.com";
            var mockUpdateRequest = new OCOPPartnerUpdateRequest
            {
                Email = mockEmail,
                ConfirmPassword = "validpassword"
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                    null
                )).ReturnsAsync((OcopPartner)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _partnerService.UpdatePartner(mockUpdateRequest));
            Assert.Equal(ResponseMessage.UserEmailNotFound, exception.Message);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.Update(It.IsAny<OcopPartner>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }
        [Fact]
        public async Task DeletePartner_ShouldReturnSuccess_WhenPartnerExists()
        {
            // Arrange
            var mockId = 1;
            var existingPartner = new OcopPartner
            {
                Id = mockId,
                Email = "partner@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                    null
                )).ReturnsAsync(existingPartner);

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SoftDelete(existingPartner));
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _partnerService.DeletePartner(mockId);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                null), Times.Once);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.SoftDelete(existingPartner), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeletePartner_ShouldThrowKeyNotFoundException_WhenPartnerDoesNotExist()
        {
            // Arrange
            var mockId = 1;

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                    null
                )).ReturnsAsync((OcopPartner)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _partnerService.DeletePartner(mockId));
            Assert.Equal(ResponseMessage.PartnerIdNotFound, exception.Message);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<OcopPartner, bool>>>(),
                null), Times.Once);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.SoftDelete(It.IsAny<OcopPartner>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Never);
        }
        [Fact]
        public async Task GetPartnerRevenue_ShouldReturnRevenueList_WhenDataExists()
        {
            // Arrange
            int mockPartnerId = 1;
            int? mockDay = null, mockMonth = 11, mockYear = 2024;

            var mockOrders = new List<Order>
    {
        new Order
        {
            Id = 1,
            OcopPartnerId = mockPartnerId,
            Status = Constants.Status.Completed,
            CreateDate = new DateTime(2024, 11, 1),
            TotalPrice = 100
        },
        new Order
        {
            Id = 2,
            OcopPartnerId = mockPartnerId,
            Status = Constants.Status.Completed,
            CreateDate = new DateTime(2024, 11, 2),
            TotalPrice = 200
        },
        new Order
        {
            Id = 3,
            OcopPartnerId = mockPartnerId,
            Status = Constants.Status.Canceled,
            CreateDate = new DateTime(2024, 11, 3),
            TotalPrice = 0
        },
        new Order
        {
            Id = 4,
            OcopPartnerId = mockPartnerId,
            Status = Constants.Status.Shipping,
            CreateDate = new DateTime(2024, 11, 4),
            TotalPrice = 50
        }
    };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(u => u.OrderRepo.GetQueryable(It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(mockOrders);

            // Act
            var result = await _partnerService.GetPartnerRevenue(mockPartnerId, mockDay, mockMonth, mockYear);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);

            var completedRevenue = result.FirstOrDefault(r => r.orderStatus == Constants.Status.Completed);
            Assert.NotNull(completedRevenue);
            Assert.Equal(2, completedRevenue.orderAmount);
            Assert.Equal(300, completedRevenue.amount);

            var canceledRevenue = result.FirstOrDefault(r => r.orderStatus == Constants.Status.Canceled);
            Assert.NotNull(canceledRevenue);
            Assert.Equal(1, canceledRevenue.orderAmount);
            Assert.Equal(0, canceledRevenue.amount);

            var shippingRevenue = result.FirstOrDefault(r => r.orderStatus == Constants.Status.Shipping);
            Assert.NotNull(shippingRevenue);
            Assert.Equal(1, shippingRevenue.orderAmount);
            Assert.Equal(50, shippingRevenue.amount);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.OrderRepo.GetQueryable(It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()), Times.Once);
        }

        [Fact]
        public async Task GetPartnerRevenue_ShouldThrowKeyNotFoundException_WhenPartnerDoesNotExist()
        {
            // Arrange
            int mockPartnerId = 1;

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _partnerService.GetPartnerRevenue(mockPartnerId, null, null, null));
            Assert.Equal(ResponseMessage.PartnerIdNotFound, exception.Message);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.OrderRepo.GetQueryable(It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()), Times.Never);
        }

        [Fact]
        public async Task GetPartnerRevenue_ShouldFilterOrders_ByYearMonthAndDay()
        {
            // Arrange
            int mockPartnerId = 1;
            int? mockDay = 1, mockMonth = 11, mockYear = 2024;

            var mockOrders = new List<Order>
    {
        new Order
        {
            Id = 1,
            OcopPartnerId = mockPartnerId,
            Status = Constants.Status.Completed,
            CreateDate = new DateTime(2024, 11, 1),
            TotalPrice = 100
        },
        new Order
        {
            Id = 2,
            OcopPartnerId = mockPartnerId,
            Status = Constants.Status.Completed,
            CreateDate = new DateTime(2024, 11, 2),
            TotalPrice = 200
        },
        new Order
        {
            Id = 3,
            OcopPartnerId = mockPartnerId,
            Status = Constants.Status.Completed,
            CreateDate = new DateTime(2024, 10, 1),
            TotalPrice = 150
        }
    };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(u => u.OrderRepo.GetQueryable(It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .ReturnsAsync(mockOrders);

            // Act
            var result = await _partnerService.GetPartnerRevenue(mockPartnerId, mockDay, mockMonth, mockYear);

            // Assert
            Assert.NotNull(result);
            var completedRevenue = result.FirstOrDefault(r => r.orderStatus == Constants.Status.Completed);
            Assert.NotNull(completedRevenue);
            Assert.Equal(1, completedRevenue.orderAmount);
            Assert.Equal(100, completedRevenue.amount);

            _mockUnitOfWork.Verify(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.OrderRepo.GetQueryable(It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()), Times.Once);
        }
        [Fact]
        public async Task GetPartnerTotalProduct_ShouldReturnTotalProduct_WhenDataExists()
        {
            // Arrange
            int mockPartnerId = 1;
            int? mockWarehouseId = null; // Test without warehouse filter

            var mockLots = new List<Lot>
            {
                new Lot
                {
                    ProductId = 1,
                    Product = new Product { Id = 1, Name = "Product A", OcopPartnerId = mockPartnerId },
                    TotalProductAmount = 10,
                    IsDeleted = false,
                    Inventory = new Inventory { WarehouseId = 1, Warehouse = new Warehouse { Id = 1, Name = "Warehouse 1" } }
                },
                new Lot
                {
                    ProductId = 2,
                    Product = new Product { Id = 2, Name = "Product B", OcopPartnerId = mockPartnerId },
                    TotalProductAmount = 5,
                    IsDeleted = false,
                    Inventory = new Inventory { WarehouseId = 1, Warehouse = new Warehouse { Id = 1, Name = "Warehouse 1" } }
                },
                new Lot
                {
                    ProductId = 1,
                    Product = new Product { Id = 1, Name = "Product A", OcopPartnerId = mockPartnerId },
                    TotalProductAmount = 15,
                    IsDeleted = false,
                    Inventory = new Inventory { WarehouseId = 2, Warehouse = new Warehouse { Id = 2, Name = "Warehouse 2" } }
                }
            };

            _mockUnitOfWork.Setup(u => u.LotRepo.GetQueryable(It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync(mockLots);

            // Act
            var result = await _partnerService.GetPartnerTotalProduct(mockPartnerId, mockWarehouseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(30, result.totalProductAmount);
            Assert.Equal(3, result.Products.Count);

            var productA = result.Products.FirstOrDefault(p => p.ProductName == "Product A");
            Assert.NotNull(productA);
            Assert.Equal(15, productA.stock); // 10 + 15 for Product A

            var productB = result.Products.FirstOrDefault(p => p.ProductName == "Product B");
            Assert.NotNull(productB);
            Assert.Equal(5, productB.stock); // 5 for Product B

            _mockUnitOfWork.Verify(u => u.LotRepo.GetQueryable(It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()), Times.Once);
        }

        [Fact]
        public async Task GetPartnerTotalProduct_ShouldReturnFilteredProduct_WhenWarehouseIdIsProvided()
        {
            // Arrange
            int mockPartnerId = 1;
            int mockWarehouseId = 1; // Test with specific warehouse filter

            var mockLots = new List<Lot>
    {
        new Lot
        {
            ProductId = 1,
            Product = new Product { Id = 1, Name = "Product A", OcopPartnerId = mockPartnerId },
            TotalProductAmount = 10,
            IsDeleted = false,
            Inventory = new Inventory { WarehouseId = 1, Warehouse = new Warehouse { Id = 1, Name = "Warehouse 1" } }
        },
        new Lot
        {
            ProductId = 2,
            Product = new Product { Id = 2, Name = "Product B", OcopPartnerId = mockPartnerId },
            TotalProductAmount = 5,
            IsDeleted = false,
            Inventory = new Inventory { WarehouseId = 2, Warehouse = new Warehouse { Id = 2, Name = "Warehouse 2" } }
        },
        new Lot
        {
            ProductId = 1,
            Product = new Product { Id = 1, Name = "Product A", OcopPartnerId = mockPartnerId },
            TotalProductAmount = 15,
            IsDeleted = false,
            Inventory = new Inventory { WarehouseId = 1, Warehouse = new Warehouse { Id = 1, Name = "Warehouse 1" } }
        }
    };

            _mockUnitOfWork.Setup(u => u.LotRepo.GetQueryable(It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync(mockLots);

            // Act
            var result = await _partnerService.GetPartnerTotalProduct(mockPartnerId, mockWarehouseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(25, result.totalProductAmount); // 10 from Warehouse 1 + 15 from Warehouse 1
            Assert.Single(result.Products); // Only one product, Product A, from Warehouse 1

            var productA = result.Products.FirstOrDefault(p => p.ProductName == "Product A");
            Assert.NotNull(productA);
            Assert.Equal(25, productA.stock); // 10 + 15 for Product A from Warehouse 1

            _mockUnitOfWork.Verify(u => u.LotRepo.GetQueryable(It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()), Times.Once);
        }

        [Fact]
        public async Task GetPartnerTotalProduct_ShouldThrowKeyNotFoundException_WhenNoLotsFoundForPartner()
        {
            // Arrange
            int mockPartnerId = 1;
            int? mockWarehouseId = null; // Test without warehouse filter

            _mockUnitOfWork.Setup(u => u.LotRepo.GetQueryable(It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync(new List<Lot>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _partnerService.GetPartnerTotalProduct(mockPartnerId, mockWarehouseId));
            Assert.Equal(ResponseMessage.PartnerIdNotFound, exception.Message);

            _mockUnitOfWork.Verify(u => u.LotRepo.GetQueryable(It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()), Times.Once);
        }


    }
}
