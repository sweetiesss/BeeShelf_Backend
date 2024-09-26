using Amazon;
using Amazon.S3;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Api.Authentication;
using BeeStore_Repository;
using BeeStore_Repository.Data;
using BeeStore_Repository.Logger.GlobalExceptionHandler;
using BeeStore_Repository.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySqlX.XDevAPI;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    var configuration = builder.Configuration
                        .GetSection("ConnectionStrings")
                        .Get<AppConfiguration>() 
                        ?? throw new ArgumentNullException("Configuration can not be configured.");
    builder.Services.AddInfrastructuresService(configuration.DatabaseConnection);

    builder.Services.AddJwtAuthentication(builder.Configuration);

}


if (builder.Environment.IsProduction())
{
    var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL").Value!.ToString();

    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultURL),
        new EnvironmentCredential()
    );

    var client = new SecretClient(new Uri(keyVaultURL), new EnvironmentCredential());
    var dbConnectionSecret = client.GetSecret("DBConnection");
    var s3AccessKey = client.GetSecret("BeeStore-S3-AccessKey");
    var s3SecretKey = client.GetSecret("BeeStore-S3-SecretKey");
    var s3BucketName = client.GetSecret("BeeStore-BucketName");
    var s3BucketUrl = client.GetSecret("BeeStore-S3-BucketURL");

    if (dbConnectionSecret.Value != null)
    {
        builder.Services.AddInfrastructuresService(dbConnectionSecret.Value.Value);
    }
    else
    {
        throw new InvalidOperationException("DBConnection secret is missing in Key Vault.");
    }

    builder.Services.AddJwtAuthenticationProduction(client);

    builder.Services.AddTransient<IPictureService>(_ =>
    new PictureService(
        new AmazonS3Client(s3AccessKey.Value.Value, s3SecretKey.Value.Value, RegionEndpoint.APNortheast1),
        s3BucketName.Value.Value,
        s3BucketUrl.Value.Value
    ));

}


builder.Services.AddWebAPIService();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfiguration>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();
if (builder.Environment.IsProduction()) 
{ 
    //app.UseMiddleware<ApiKeyAuthMiddleware>();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
