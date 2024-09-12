using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Api.Authentication;
using BeeStore_Repository.Data;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsProduction())
{
    var configuration = builder.Configuration.GetSection("ConnectionStrings").Get<AppConfiguration>();
    builder.Services.AddInfrastructuresService(configuration.DatabaseConnection);
    builder.Services.AddWebAPIService();
}

if (builder.Environment.IsDevelopment())
{
    var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL").Value!.ToString();
    var keyVaultClientId = builder.Configuration.GetSection("KeyVault:ClientId").Value!.ToString();
    var keyVaultClientSecret = builder.Configuration.GetSection("KeyVault:ClientSecret").Value!.ToString();
    var keyVaultDirectoryId = builder.Configuration.GetSection("KeyVault:DirectoryID").Value!.ToString();

    if (string.IsNullOrEmpty(keyVaultURL) || string.IsNullOrEmpty(keyVaultClientId) ||
        string.IsNullOrEmpty(keyVaultClientSecret) || string.IsNullOrEmpty(keyVaultDirectoryId))
    {
        throw new InvalidOperationException("Key Vault configuration values are missing.");
    }

    var credential = new ClientSecretCredential(
        keyVaultDirectoryId,
        keyVaultClientId,
        keyVaultClientSecret
    );

    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultURL),
        credential
    );

    var client = new SecretClient(new Uri(keyVaultURL), credential);
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

app.UseAuthorization();

app.MapControllers();


app.Run();
