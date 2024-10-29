using BeeStore_Api.Controllers;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Api_Test.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IJWTService> _mockJwtService;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockJwtService = new Mock<IJWTService>();
            _userController = new UserController(_mockUserService.Object, _mockJwtService.Object);
        }

        [Fact]
        public async Task GetEmployees_ShouldReturnEmployeeList()
        {
            // Arrange
            var emplist = new Pagination<EmployeeListDTO>();

            _mockUserService.Setup(service => service.GetAllEmployees(null, null, null, false, 0, 10))
                .ReturnsAsync(emplist);

            // Act
            var response = await _userController.GetEmployees(null, null, null, false, 0, 10) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(emplist, response.Value);
        }

        [Fact]
        public async Task GetEmployee_WithValidEmail_ShouldReturnEmployee()
        {
            // Arrange
            var email = "employee@example.com";
            var employee = new EmployeeListDTO
            {
                Id = 1,
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                RoleName = "Manager"
            };

            _mockUserService.Setup(service => service.GetEmployee(email))
                .ReturnsAsync(employee);

            // Act
            var response = await _userController.GetEmployee(email) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(employee, response.Value);
        }

        [Fact]
        public async Task CreateEmployee_ShouldReturnCreatedEmployee()
        {
            // Arrange
            var newEmployeeRequest = new EmployeeCreateRequest
            {
                Email = "newemployee@example.com",
                Password = "Password123",
                FirstName = "New",
                LastName = "Employee",
                RoleId = 2
            };

            _mockUserService.Setup(service => service.CreateEmployee(newEmployeeRequest))
                .ReturnsAsync(ResponseMessage.Success);

            // Act
            var response = await _userController.CreateEmployee(newEmployeeRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task UpdateEmployee_ShouldReturnUpdatedEmployee()
        {
            // Arrange
            var updateEmployeeRequest = new EmployeeUpdateRequest
            {
                Email = "existingemployee@example.com",
                ConfirmPassword = "Password123",
                FirstName = "Updated",
                LastName = "Employee"
            };

            _mockUserService.Setup(service => service.UpdateEmployee(updateEmployeeRequest))
                .ReturnsAsync(ResponseMessage.Success);

            // Act
            var response = await _userController.UpdateEmployee(updateEmployeeRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public async Task DeleteEmployee_ShouldReturnSuccessMessage()
        {
            // Arrange
            var employeeId = 1;
            var deleteMessage = "Employee deleted successfully.";
            _mockUserService.Setup(service => service.DeleteEmployee(employeeId))
                .ReturnsAsync(deleteMessage);

            // Act
            var response = await _userController.DeleteEmployee(employeeId) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(deleteMessage, response.Value);
        }
    }
}
