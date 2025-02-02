﻿using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BeeStore_Repository.Services
{
    public class JWTService : IJWTService
    {
        private readonly string _keyVaultURL;
        public SecretClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JWTService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _keyVaultURL = configuration["KeyVault:KeyVaultURL"] ?? throw new ArgumentNullException("Key Vault URL configuration values are missing.");
            _client = new SecretClient(new Uri(_keyVaultURL), new EnvironmentCredential());
            _httpContextAccessor = httpContextAccessor;
        }

        public string RefreshJWTToken(UserRefreshTokenRequestDTO jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RetrieveJWTSecretKey()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var principal = tokenHandler.ValidateToken(jwt.Jwt, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = RetrieveJWTIssuer(),
                ValidAudience = RetrieveJWTAudience(),
                ValidateLifetime = false,
            }, out SecurityToken validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;
            if (jwtToken == null || jwtToken.ValidTo > DateTime.UtcNow)
            {
                throw new SecurityTokenException(ResponseMessage.JwtTokenHasNotExpired);
            }

            var userEmail = jwtToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Email);
            var userRole = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Role);

            var claims = GenerateClaim(userEmail.Value, userRole.Value);
            var newToken = GenerateToken(creds, claims);

            return tokenHandler.WriteToken(newToken);
        }


        public string GenerateJwtToken(string userEmail, string userRole)
        {
            if (userEmail is null)
            {
                throw new ArgumentNullException(nameof(userEmail));
            }

            if (userRole is null)
            {
                throw new ArgumentNullException(nameof(userRole));
            }

            var claims = GenerateClaim(userEmail, userRole);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RetrieveJWTSecretKey()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = GenerateToken(creds, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        private string RetrieveJWTSecretKey()
        {
            var secretResponse = _client.GetSecret("BeeStore-JWT-SecretKey");

            if (string.IsNullOrEmpty(secretResponse.Value.Value))
            {
                throw new InvalidOperationException(ResponseMessage.JWTSecretKeyError);
            }


            return secretResponse.Value.Value;
        }
        private string RetrieveJWTIssuer()
        {
            var secretResponse = _client.GetSecret("BeeStore-JWT-Issuer");

            if (string.IsNullOrEmpty(secretResponse.Value.Value))
            {
                throw new InvalidOperationException(ResponseMessage.JWTIssuerValueError);
            }

            return secretResponse.Value.Value;
        }

        private string RetrieveJWTAudience()
        {
            var secretResponse = _client.GetSecret("BeeStore-JWT-Audience");

            if (string.IsNullOrEmpty(secretResponse.Value.Value))
            {
                throw new InvalidOperationException(ResponseMessage.JwtAudienceValueError);
            }

            return secretResponse.Value.Value;
        }

        private Claim[] GenerateClaim(string userEmail, string userRole)
        {
            return new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, userEmail),
                new Claim(ClaimTypes.Role, userRole),
                new Claim(JwtRegisteredClaimNames.Sub, userEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
        }

        private JwtSecurityToken GenerateToken(SigningCredentials creds, Claim[] claims)
        {
            return new JwtSecurityToken(
                    issuer: RetrieveJWTIssuer(),
                    audience: RetrieveJWTAudience(),
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(24 * 60),
                    signingCredentials: creds);
        }


        public async Task<string> GetUserEmail()
        {
            var tokenContent = await ExtractTokenContent();
            return tokenContent.Email;
        }

        public async Task<string> GetUserRole()
        {
            var tokenContent = await ExtractTokenContent();
            return tokenContent.Role;
        }

        private async Task<string> GetBearerTokenFromHttpContext()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
                return null;

            const string bearerPrefix = "Bearer ";
            if (authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return authHeader[bearerPrefix.Length..].Trim();
            }

            return authHeader.Trim();
        }

        private async Task<TokenContent> ExtractTokenContent()
        {
            var token = await GetBearerTokenFromHttpContext();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("No token found in the request");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            return new TokenContent
            {
                Email = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value,
                Role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
            };
        }
    }
}
