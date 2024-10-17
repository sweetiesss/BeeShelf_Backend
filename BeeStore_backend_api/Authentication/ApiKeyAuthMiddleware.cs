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
            return providedApiKey.Equals(RetrieveApiKey());
        }

        private string RetrieveApiKey()
        {

            var keyVaultURL = _configuration.GetValue<string>(AuthConstant.KeyVaultUrl);


            if (string.IsNullOrEmpty(keyVaultURL))
            {
                throw new InvalidOperationException("Key Vault configuration values are missing.");
            }

            var client = new SecretClient(new Uri(keyVaultURL), new EnvironmentCredential());

            var secretResponse = client.GetSecret("BeeStore-Apikey");

            return secretResponse.Value.Value;
        }

    }
}
