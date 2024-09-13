using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Api.Authentication;
using BeeStore_Repository.Data;
using BeeStore_Repository.Logger.GlobalExceptionHandler;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsDevelopment())
{
    var configuration = builder.Configuration.Get<AppConfiguration>();
    builder.Services.AddInfrastructuresService(configuration.DatabaseConnection);
    builder.Services.AddWebAPIService();
}

if (builder.Environment.IsProduction())
{
    var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL").Value!.ToString();

    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback));

    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultURL),
        new DefaultAzureCredential()
    );

    var client = new SecretClient(new Uri(keyVaultURL), new DefaultAzureCredential());
    var dbConnectionSecret = client.GetSecret("DBConnection");

    if (dbConnectionSecret.Value != null)
    {
        builder.Services.AddInfrastructuresService(dbConnectionSecret.Value.Value);
    }
    else
    {
        throw new InvalidOperationException("DBConnection secret is missing in Key Vault.");
    }
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();


app.Run();
