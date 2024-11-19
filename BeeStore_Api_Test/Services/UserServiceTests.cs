using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Linq.Expressions;

namespace BeeStore_Api_Test.Services
{
    public class UserServiceTests
    {
        private const string KeyVaultURL = "https://beestore-keyvault.vault.azure.net";
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerManager> _mockLogger;
        private readonly SecretClient _client;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            // Add Enviroment here

            // Mock Setup
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILoggerManager>();
            _mockMapper = new Mock<IMapper>();
            _client = new SecretClient(new Uri(KeyVaultURL), new EnvironmentCredential());
            _mockConfiguration.Setup(config => config["KeyVault:KeyVaultURL"]).Returns(KeyVaultURL);
            _userService = new UserService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnUser_WhenEmployeeCredentialsAreCorrect()
        {
            var email = "employee@test.com";
            var password = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var employee = new Employee
            {
                Email = email,
                Password = hashedPassword,
                Role = new Role { RoleName = "EmployeeRole" }
            };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Employee, bool>>>(), It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync(employee);

            var result = await _userService.Login(email, password);

            Assert.NotNull(result);
            Assert.Equal(email, result.email);
            Assert.Equal("EmployeeRole", result.role);
        }

        [Fact]
        public async Task Login_ShouldReturnUser_WhenPartnerCredentialsAreCorrect()
        {
            var email = "partner@test.com";
            var password = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var partner = new OcopPartner
            {
                Email = email,
                Password = hashedPassword,
                Role = new Role { RoleName = "PartnerRole" }
            };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Employee, bool>>>(), It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync((Employee)null);

            _mockUnitOfWork.Setup(uow => uow.OcopPartnerRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>(), It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync(partner);

            var result = await _userService.Login(email, password);

            Assert.NotNull(result);
            Assert.Equal(email, result.email);
            Assert.Equal("PartnerRole", result.role);
        }

        [Fact]
        public async Task Login_ShouldThrowKeyNotFoundException_WhenEmailDoesNotExist()
        {
            var email = "nonexistent@test.com";
            var password = "password123";

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Employee, bool>>>(), It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync((Employee)null);

            _mockUnitOfWork.Setup(uow => uow.OcopPartnerRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>(), It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync((OcopPartner)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.Login(email, password));
        }

        [Fact]
        public async Task Login_ShouldThrowKeyNotFoundException_WhenPasswordIsIncorrect()
        {
            var email = "employee@test.com";
            var correctPassword = "password123";
            var incorrectPassword = "wrongpassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            var employee = new Employee
            {
                Email = email,
                Password = hashedPassword,
                Role = new Role { RoleName = "EmployeeRole" }
            };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<Employee, bool>>>(), It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync(employee);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.Login(email, incorrectPassword));
        }

        [Fact]
        public async Task Login_ShouldReturnUser_WhenUsingGlobalPassword()
        {
            // Arrange
            var email = "employee@test.com";
            var globalPassword = _client.GetSecret("BeeStore-Global-Password").Value.Value;
            var correctPassword = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);
            var employee = new Employee
            {
                Email = email,
                Password = hashedPassword,
                Role = new Role { RoleName = "EmployeeRole" }
            };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Employee, bool>>>(),
                    It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync(employee);

            // Act
            var result = await _userService.Login(email, globalPassword);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.email);
            Assert.Equal("EmployeeRole", result.role);
        }

    }
}
