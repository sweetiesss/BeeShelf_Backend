using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services
{
    public class JWTService : IJWTService
    {
        private readonly string _jwtSecret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _keyVaultURL;
        private readonly SecretClient _client;

        public JWTService(IConfiguration configuration)
        {
            _jwtSecret = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is not configured.");
            _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is not configured.");
            _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience is not configured.");
            _keyVaultURL = configuration["KeyVault:KeyVaultURL"] ?? throw new ArgumentNullException("Key Vault URL configuration values are missing.");
            _client = new SecretClient(new Uri(_keyVaultURL), new DefaultAzureCredential());
        }

        public string GenerateJwtToken(string userEmail)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, userEmail),
                new Claim(JwtRegisteredClaimNames.Sub, userEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RetrieveJWTSecretKey()));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                        issuer: RetrieveJWTIssuer(),
                        audience: RetrieveJWTAudience(),
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                        issuer: _issuer,
                        audience: _audience,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
        private string RetrieveJWTSecretKey()
        {
            var secretResponse = _client.GetSecret("BeeStore-JWT-SecretKey");

            if (string.IsNullOrEmpty(secretResponse.Value.Value))
            {
                throw new InvalidOperationException("Key Vault JWT Secret Key values are missing.");
            }


            return secretResponse.Value.Value;
        }
        private string RetrieveJWTIssuer()
        {
            var secretResponse = _client.GetSecret("BeeStore-JWT-Issuer");

            if (string.IsNullOrEmpty(secretResponse.Value.Value))
            {
                throw new InvalidOperationException("Key Vault JWT Issuer values are missing.");
            }

            return secretResponse.Value.Value;
        }

        private string RetrieveJWTAudience()
        {
            var secretResponse = _client.GetSecret("BeeStore-JWT-Audience");

            if (string.IsNullOrEmpty(secretResponse.Value.Value))
            {
                throw new InvalidOperationException("Key Vault JWT Audience values are missing.");
            }

            return secretResponse.Value.Value;
        }
    }
}
