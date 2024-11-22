using AutoMapper;
using BeeStore_Repository;
using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services;
using BeeStore_Repository.Utils;
using Moq;
using System.Linq.Expressions;

namespace BeeStore_Api_Test.Services
{
    public class LotServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerManager> _mockLogger;
        private readonly LotService _lotService;

        public LotServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerManager>();
            _lotService = new LotService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task DeleteLot_ShouldReturnSuccess_WhenLotExists()
        {
            // Arrange
            int lotId = 1;
            var lot = new Lot { Id = lotId };
            _mockUnitOfWork.Setup(u => u.LotRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Lot, bool>>>(), It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync(lot);

            // Act
            var result = await _lotService.DeleteLot(lotId);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(u => u.LotRepo.SoftDelete(It.IsAny<Lot>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteLot_ShouldThrowKeyNotFoundException_WhenLotDoesNotExist()
        {
            // Arrange
            int lotId = 1;
            _mockUnitOfWork.Setup(u => u.LotRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Lot, bool>>>(), It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync((Lot)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _lotService.DeleteLot(lotId));
            Assert.Equal(ResponseMessage.PackageIdNotFound, exception.Message);
        }

        [Fact]
        public async Task GetLotById_ShouldReturnLot_WhenLotExists()
        {
            // Arrange
            int lotId = 1;
            var lot = new Lot { Id = lotId };
            var lotDto = new LotListDTO { Id = lotId };

            _mockUnitOfWork.Setup(u => u.LotRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Lot, bool>>>(), It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync(lot);
            _mockMapper.Setup(m => m.Map<LotListDTO>(lot)).Returns(lotDto);

            // Act
            var result = await _lotService.GetLotById(lotId);

            // Assert
            Assert.Equal(lotDto, result);
        }

        [Fact]
        public async Task GetLotById_ShouldThrowKeyNotFoundException_WhenLotDoesNotExist()
        {
            // Arrange
            int lotId = 1;
            _mockUnitOfWork.Setup(u => u.LotRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Lot, bool>>>(), It.IsAny<Func<IQueryable<Lot>, IQueryable<Lot>>>()))
                .ReturnsAsync((Lot)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _lotService.GetLotById(lotId));
            Assert.Equal(ResponseMessage.PackageIdNotFound, exception.Message);
        }

        [Fact]
        public async Task GetAllLots_ShouldReturnPaginatedLots()
        {
            // Arrange
            var lots = new List<Lot> { new Lot { Id = 1 }, new Lot { Id = 2 } };
            var lotsDto = new List<LotListDTO>
            {
                new LotListDTO { Id = 1 },
                new LotListDTO { Id = 2 }
            };

            _mockUnitOfWork.Setup(u => u.LotRepo.GetListAsync(
                    It.IsAny<Expression<Func<Lot, bool>>>(),
                    null,
                    null,
                    false,
                    It.IsAny<string>(),
                    It.IsAny<Expression<Func<Lot, string>>[]>()))
                .ReturnsAsync(lots);

            _mockMapper.Setup(m => m.Map<List<LotListDTO>>(lots)).Returns(lotsDto);

            // Act
            var result = await _lotService.GetAllLots(null, null, null, null, false, 0, 10);

            // Assert
            Assert.Equal(2, result.TotalItemsCount);
            Assert.Equal(lotsDto, result.Items);
        }

        [Fact]
        public async Task GetLotsByUserId_ShouldThrowKeyNotFoundException_WhenPartnerDoesNotExist()
        {
            // Arrange
            int partnerId = 1;
            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _lotService.GetLotsByUserId(partnerId, null, null, null, null, false, 0, 10));
            Assert.Equal(ResponseMessage.UserIdNotFound, exception.Message);
        }

        [Fact]
        public async Task GetLotsByUserId_ShouldReturnPaginatedLots_WhenValidPartnerId()
        {
            // Arrange
            int partnerId = 1;
            var lots = new List<Lot> { new Lot { Id = 1 }, new Lot { Id = 2 } };
            var lotsDto = new List<LotListDTO>
            {
                new LotListDTO { Id = 1 },
                new LotListDTO { Id = 2 }
            };

            _mockUnitOfWork.Setup(u => u.OcopPartnerRepo.AnyAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>()))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(u => u.LotRepo.GetListAsync(
                    It.IsAny<Expression<Func<Lot, bool>>>(),
                    null,
                    null,
                    false,
                    It.IsAny<string>(),
                    It.IsAny<Expression<Func<Lot, string>>[]>()))
                .ReturnsAsync(lots);

            _mockMapper.Setup(m => m.Map<List<LotListDTO>>(lots)).Returns(lotsDto);

            // Act
            var result = await _lotService.GetLotsByUserId(partnerId, null, null, null, null, false, 0, 10);

            // Assert
            Assert.Equal(2, result.TotalItemsCount);
            Assert.Equal(lotsDto, result.Items);
        }
    }

}
