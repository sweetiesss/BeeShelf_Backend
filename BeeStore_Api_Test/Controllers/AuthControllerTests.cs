using BeeStore_Api.Controllers;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BeeStore_Api_Test.Controllers
{
    public class AuthControllerTests
    {

        private readonly Mock<IJWTService> _mockJwtService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockJwtService = new Mock<IJWTService>();
            _mockUserService = new Mock<IUserService>();
            _authController = new AuthController(_mockJwtService.Object, _mockUserService.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsJwtToken()
        {
            // Arrange
            var request = new UserLoginRequestDTO { email = "test@example.com", password = "password123" };
            var userResult = new UserLoginResponseDTO(request.email, "Partner");
            var token = "generated_jwt_token";

            _mockUserService.Setup(s => s.Login(request.email, request.password))
                .ReturnsAsync(userResult);
            _mockJwtService.Setup(s => s.GenerateJwtToken(userResult.email, userResult.role))
                .Returns(token);

            // Act
            var result = await _authController.Login(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(token, result.Value);
        }
        [Fact]
        public async Task Login_InvalidCredentials_ReturnUnauthorized()
        {
            // Arrange
            var invalidRequest = new UserLoginRequestDTO
            {
                email = "invalid@example.com",
                password = "wrongpassword"
            };

            _mockUserService
                .Setup(service => service.Login(invalidRequest.email, invalidRequest.password))
                .ReturnsAsync((UserLoginResponseDTO)null);

            // Act
            var response = await _authController.Login(invalidRequest);

            // Assert
            Assert.IsType<UnauthorizedResult>(response);
        }

        [Fact]
        public async Task Signup_WithValidUserDetails_ShouldReturnSuccessMessage()
        {
            // Arrange
            var signUpRequest = new UserSignUpRequestDTO
            {
                Email = "newuser@example.com",
                //Password = "NewUserPassword",
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890",
                CitizenIdentificationNumber = "123456789",
                TaxIdentificationNumber = "987654321",
                BusinessName = "John's Shop",
                BankName = "Test Bank",
                BankAccountNumber = "123456789",
                PictureLink = "http://example.com/profile.jpg"
            };

            _mockUserService.Setup(service => service.SignUp(signUpRequest))
                .ReturnsAsync(ResponseMessage.Success);

            // Act
            var response = await _authController.Signup(signUpRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ResponseMessage.Success, response.Value);
        }

        [Fact]
        public void RefreshToken_WithValidJwt_ShouldReturnRefreshedToken()
        {
            // Arrange
            var refreshRequest = new UserRefreshTokenRequestDTO
            {
                Jwt = "expired_jwt_token"
            };
            var refreshedToken = "refreshed_jwt_token";

            _mockJwtService.Setup(service => service.RefreshJWTToken(refreshRequest))
                .Returns(refreshedToken);

            // Act
            var response = _authController.RefreshToken(refreshRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(refreshedToken, response.Value);
        }
    }
}
