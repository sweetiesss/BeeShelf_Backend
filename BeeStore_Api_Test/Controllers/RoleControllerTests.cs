using BeeStore_Api.Controllers;
using BeeStore_Repository.DTO.RoleDTOs;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Api_Test.Controllers
{
    public class RoleControllerTests
    {
        private readonly Mock<IRoleService> _mockRoleService;
        private readonly RoleController _roleController;

        public RoleControllerTests()
        {
            _mockRoleService = new Mock<IRoleService>();
            _roleController = new RoleController(_mockRoleService.Object);
        }

        [Fact]
        public async Task GetRoles_ShouldReturnRoleList()
        {
            // Arrange
            var roleList = new List<RoleListDTO>();

            _mockRoleService.Setup(service => service.GetRoles())
                .ReturnsAsync(roleList);

            // Act
            var response = await _roleController.GetRoles() as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(roleList, response.Value);
        }

        [Fact]
        public async Task UpdateUserRole_ShouldReturnSuccessMessage()
        {
            // Arrange
            int userId = 1;
            string roleName = "Manager";

            _mockRoleService.Setup(service => service.UpdateUserRole(userId, roleName))
                .ReturnsAsync(ResponseMessage.Success);

            // Act
            var response = await _roleController.UpdateUserRole(userId, roleName) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }
    }
}
