using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Services;
using BeeStore_Repository.Utils;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlX.XDevAPI;

namespace BeeStore_Api_Test.Services
{
    public class JWTServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<SecretClient> _mockSecretClient;
        private readonly JWTService _jwtService;
        private const string JwtSecretKey = "TestSecretKey12345678901234567890"; // At least 32 characters
        private const string JwtIssuer = "TestIssuer";
        private const string JwtAudience = "TestAudience";

        public JWTServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["KeyVault:KeyVaultURL"]).Returns("https://fake-key-vault.vault.azure.net/");

            _mockSecretClient = new Mock<SecretClient>(MockBehavior.Strict, new Uri("https://fake-key-vault.vault.azure.net/"), new EnvironmentCredential());

            // Mock SecretClient for retrieving secrets
            _mockSecretClient
                .Setup(client => client.GetSecret("JWTSecretKey", null, It.IsAny<CancellationToken>()))
                .Returns(CreateMockKeyVaultResponse("JWTSecretKey", JwtSecretKey));

            _mockSecretClient
                .Setup(client => client.GetSecret("JWTIssuer", null, It.IsAny<CancellationToken>()))
                .Returns(CreateMockKeyVaultResponse("JWTIssuer", JwtIssuer));

            _mockSecretClient
                .Setup(client => client.GetSecret("JWTAudience", null, It.IsAny<CancellationToken>()))
                .Returns(CreateMockKeyVaultResponse("JWTAudience", JwtAudience));

            // Create an instance of JWTService with mock dependencies
            _jwtService = new JWTService(_mockConfiguration.Object)
            {
                _client = _mockSecretClient.Object // Injecting mock SecretClient
            };
        }

        private Response<KeyVaultSecret> CreateMockKeyVaultResponse(string name, string value)
        {
            var secret = new KeyVaultSecret(name, value);
            var mockResponse = new Mock<Response<KeyVaultSecret>>();
            mockResponse.Setup(r => r.Value).Returns(secret);
            return mockResponse.Object;
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnValidToken_WhenInputIsValid()
        {
            // Arrange
            string userEmail = "test@example.com";
            string userRole = "Admin";

            // Act
            var token = _jwtService.GenerateJwtToken(userEmail, userRole);

            // Assert
            Assert.NotNull(token);
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.Equal(JwtIssuer, jsonToken.Issuer);
            Assert.Contains(jsonToken.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == userEmail);
            Assert.Contains(jsonToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == userRole);
        }

        [Fact]
        public void GenerateJwtToken_ShouldThrowArgumentNullException_WhenEmailIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _jwtService.GenerateJwtToken(null, "Admin"));
        }

        [Fact]
        public void GenerateJwtToken_ShouldThrowArgumentNullException_WhenRoleIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _jwtService.GenerateJwtToken("test@example.com", null));
        }

        [Fact]
        public void RefreshJwtToken_ShouldReturnNewToken_WhenTokenIsValidAndExpired()
        {
            // Arrange
            var handler = new JwtSecurityTokenHandler();
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Email, "test@example.com"),
            new Claim(ClaimTypes.Role, "Admin")
        };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiredToken = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                notBefore: DateTime.UtcNow.AddHours(-2),
                expires: DateTime.UtcNow.AddHours(-1),
                signingCredentials: creds);

            var jwtRequest = new UserRefreshTokenRequestDTO { Jwt = handler.WriteToken(expiredToken) };

            // Act
            var newToken = _jwtService.RefreshJWTToken(jwtRequest);

            // Assert
            Assert.NotNull(newToken);
            var newTokenClaims = handler.ValidateToken(newToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = JwtIssuer,
                ValidAudience = JwtAudience,
                ValidateLifetime = true,
            }, out _);

            Assert.Equal("test@example.com", newTokenClaims.FindFirst(JwtRegisteredClaimNames.Email)?.Value);
            Assert.Equal("Admin", newTokenClaims.FindFirst(ClaimTypes.Role)?.Value);
        }

        [Fact]
        public void RefreshJwtToken_ShouldThrowSecurityTokenException_WhenTokenHasNotExpired()
        {
            // Arrange
            var handler = new JwtSecurityTokenHandler();
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Email, "test@example.com"),
            new Claim(ClaimTypes.Role, "Admin")
        };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var validToken = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            var jwtRequest = new UserRefreshTokenRequestDTO { Jwt = handler.WriteToken(validToken) };

            // Act & Assert
            var exception = Assert.Throws<SecurityTokenException>(() => _jwtService.RefreshJWTToken(jwtRequest));
            Assert.Equal(ResponseMessage.JwtTokenHasNotExpired, exception.Message);
        }
    }


}
