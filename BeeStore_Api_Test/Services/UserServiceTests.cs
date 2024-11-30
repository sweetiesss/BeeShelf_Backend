using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services;
using BeeStore_Repository.Utils;
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
        [Fact]
        public async Task GetAllEmployees_ShouldReturnPaginatedList_WhenDataExists()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Email = "admin@test.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Role = new Role { RoleName = TestConstants.RoleName.Admin },
                    CreateDate = DateTime.Now
                },
                new Employee
                {
                    Id = 2,
                    Email = "manager@test.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Role = new Role { RoleName = TestConstants.RoleName.Manager },
                    CreateDate = DateTime.Now
                }
            };

            var employeeDTOs = employees.Select(e => new EmployeeListDTO
            {
                Id = e.Id,
                Email = e.Email,
                FirstName = e.FirstName,
                LastName = e.LastName,
                RoleName = e.Role.RoleName,
                CreateDate = e.CreateDate
            }).ToList();

            var paginationResult = new Pagination<EmployeeListDTO>
            {
                PageIndex = 0,
                PageSize = 10,
                TotalItemsCount = employeeDTOs.Count,
                Items = employeeDTOs
            };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.GetListAsync(
                    It.IsAny<Expression<Func<Employee, bool>>>(),
                    It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<Expression<Func<Employee, string>>[]>()))
                .ReturnsAsync(employees);

            _mockMapper.Setup(mapper => mapper.Map<List<EmployeeListDTO>>(employees))
                .Returns(employeeDTOs);

            // Act
            var result = await _userService.GetAllEmployees(
                search: null,
                role: null,
                sortCriteria: null,
                order: false,
                pageIndex: 0,
                pageSize: 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paginationResult.TotalItemsCount, result.TotalItemsCount);
            Assert.Equal(paginationResult.Items.Count, result.Items.Count);
            Assert.Equal(paginationResult.Items.First().Email, result.Items.First().Email);
        }
        [Fact]
        public async Task GetEmployee_ShouldReturnEmployee_WhenEmailExists()
        {
            // Arrange
            var email = "employee@test.com";
            var employee = new Employee
            {
                Id = 1,
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                Role = new Role { RoleName = TestConstants.RoleName.User },
                CreateDate = DateTime.Now
            };

            var expectedDTO = new EmployeeListDTO
            {
                Id = employee.Id,
                Email = employee.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                RoleName = employee.Role.RoleName,
                CreateDate = employee.CreateDate
            };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Employee, bool>>>(),
                    It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync(employee);

            _mockMapper.Setup(mapper => mapper.Map<EmployeeListDTO>(employee))
                .Returns(expectedDTO);

            // Act
            var result = await _userService.GetEmployee(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDTO.Email, result.Email);
            Assert.Equal(expectedDTO.RoleName, result.RoleName);
        }

        [Fact]
        public async Task GetEmployee_ShouldThrowKeyNotFoundException_WhenEmailDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@test.com";

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(
                    It.IsAny<Expression<Func<Employee, bool>>>(),
                    It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync((Employee)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.GetEmployee(email));
        }
        [Fact]
        public async Task SignUp_ShouldReturnSuccess_WhenUserSignUpIsSuccessful()
        {
            // Arrange
            var request = new UserSignUpRequestDTO
            {
                Email = "newpartner@test.com",
                FirstName = "John",
                LastName = "Doe",
                Phone = "123456789",
                BusinessName = "Bee Business"
            };

            var generatedPassword = "GeneratedPassword123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

            var newPartner = new OcopPartner
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                BusinessName = request.BusinessName,
                RoleId = 2,
                Password = hashedPassword
            };

            _mockUnitOfWork.Setup(uow => uow.OcopPartnerRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>(), It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
                .ReturnsAsync((OcopPartner)null);

            _mockMapper.Setup(mapper => mapper.Map<OcopPartner>(request))
                .Returns(newPartner);

            _mockUnitOfWork.Setup(uow => uow.OcopPartnerRepo.AddAsync(It.IsAny<OcopPartner>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(uow => uow.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.SignUp(request);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(uow => uow.OcopPartnerRepo.AddAsync(It.Is<OcopPartner>(partner =>
                partner.Email == newPartner.Email &&
                partner.Password != generatedPassword && // Ensure password is hashed
                partner.RoleId == 2)), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task SignUp_ShouldThrowDuplicateException_WhenEmailAlreadyExists()
        {
            // Arrange
            var request = new UserSignUpRequestDTO
            {
                Email = "existingpartner@test.com"
            };

            var existingPartner = new OcopPartner
            {
                Email = request.Email
            };

            _mockUnitOfWork.Setup(uow => uow.OcopPartnerRepo.SingleOrDefaultAsync(It.IsAny<Expression<Func<OcopPartner, bool>>>(), It.IsAny<Func<IQueryable<OcopPartner>, IQueryable<OcopPartner>>>()))
               .ReturnsAsync(existingPartner);

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateException>(() => _userService.SignUp(request));
        }
        [Fact]
        public async Task CreateEmployee_ShouldReturnSuccess_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var request = new EmployeeCreateRequest
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
               .ReturnsAsync((Employee)null);

            _mockMapper.Setup(m => m.Map<Employee>(request)).Returns(new Employee { Email = request.Email });

            _mockUnitOfWork.Setup(u => u.EmployeeRepo.AddAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.CreateEmployee(request);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(u => u.EmployeeRepo.AddAsync(It.IsAny<Employee>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateEmployee_ShouldThrowDuplicateException_WhenEmailExists()
        {
            // Arrange
            var request = new EmployeeCreateRequest { Email = "test@example.com" };
            var existingEmployee = new Employee { Email = request.Email };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
               .ReturnsAsync(existingEmployee);

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateException>(() => _userService.CreateEmployee(request));
        }
        [Fact]
        public async Task DeleteEmployee_ShouldReturnSuccess_WhenEmployeeExists()
        {
            // Arrange
            int employeeId = 1;
            var existingEmployee = new Employee { Id = employeeId };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
               .ReturnsAsync(existingEmployee);

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.DeleteEmployee(employeeId);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(u => u.EmployeeRepo.SoftDelete(existingEmployee), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployee_ShouldThrowKeyNotFoundException_WhenEmployeeDoesNotExist()
        {
            // Arrange
            int employeeId = 1;

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
               .ReturnsAsync((Employee)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.DeleteEmployee(employeeId));
        }
        [Fact]
        public async Task UpdateEmployee_ShouldReturnSuccess_WhenPasswordMatches()
        {
            // Arrange
            var request = new EmployeeUpdateRequest
            {
                Email = "test@example.com",
                ConfirmPassword = "password",
                FirstName = "UpdatedName"
            };

            var existingEmployee = new Employee
            {
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword("password")
            };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync(existingEmployee);

            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateEmployee(request);

            // Assert
            Assert.Equal(ResponseMessage.Success, result);
            _mockUnitOfWork.Verify(u => u.EmployeeRepo.Update(existingEmployee), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldThrowKeyNotFoundException_WhenEmailDoesNotExist()
        {
            // Arrange
            var request = new EmployeeUpdateRequest { Email = "nonexistent@example.com", ConfirmPassword = "password" };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync((Employee)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.UpdateEmployee(request));
        }

        [Fact]
        public async Task UpdateEmployee_ShouldThrowApplicationException_WhenPasswordDoesNotMatch()
        {
            // Arrange
            var request = new EmployeeUpdateRequest
            {
                Email = "test@example.com",
                ConfirmPassword = "wrongpassword"
            };

            var existingEmployee = new Employee
            {
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword("correctpassword")
            };

            _mockUnitOfWork.Setup(uow => uow.EmployeeRepo.SingleOrDefaultAsync(
                It.IsAny<Expression<Func<Employee, bool>>>(),
                It.IsAny<Func<IQueryable<Employee>, IQueryable<Employee>>>()))
                .ReturnsAsync(existingEmployee);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _userService.UpdateEmployee(request));
        }

    }
}
