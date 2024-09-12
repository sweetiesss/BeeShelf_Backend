using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace BeeStore_Api.Authentication
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(AuthConstant.ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key was not provided.");
                return;
            }

            if (!IsValidApiKey(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            await _next(context);
        }

        private bool IsValidApiKey(string providedApiKey)
        {
            //var apikey = _configuration.GetValue<string>(AuthConstant.ApiKeySectionName);
            return providedApiKey.Equals(RetrieveApiKey());
        }

        private string RetrieveApiKey()
        {
            var keyVaultURL = _configuration.GetValue<string>(AuthConstant.KeyVaultUrl);
            var keyVaultClientId = _configuration.GetValue<string>(AuthConstant.ClientId);
            var keyVaultClientSecret = _configuration.GetValue<string>(AuthConstant.ClientSecret);
            var keyVaultDirectoryId = _configuration.GetValue<string>(AuthConstant.DirectoryId);


            if (string.IsNullOrEmpty(keyVaultURL) ||
                string.IsNullOrEmpty(keyVaultClientId) ||
                string.IsNullOrEmpty(keyVaultClientSecret) ||
                string.IsNullOrEmpty(keyVaultDirectoryId))
            {
                throw new InvalidOperationException("Key Vault configuration values are missing.");
            }

            var credential = new ClientSecretCredential(
                keyVaultDirectoryId,
                keyVaultClientId,
                keyVaultClientSecret
            );

            var client = new SecretClient(new Uri(keyVaultURL), credential);

            var secretResponse = client.GetSecret("BeeStore-Apikey");

            return secretResponse.Value.Value;
        }

    }
}
