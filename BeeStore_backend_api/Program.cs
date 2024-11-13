using Amazon;
using Amazon.S3;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Api.Authentication;
using BeeStore_Repository;
using BeeStore_Repository.Data;
using BeeStore_Repository.Logger.GlobalExceptionHandler;
using BeeStore_Repository.Services;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils.Validator.BatchDTOs;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

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
    var keyVaultURL = builder.Configuration
                             .GetSection("KeyVault")
                             .Get<AppConfiguration>();

    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultURL.KeyVaultURL),
        new EnvironmentCredential()
    );

    var client = new SecretClient(new Uri(keyVaultURL.KeyVaultURL), new EnvironmentCredential());

    var dbConnectionSecret = client.GetSecret("DBConnection");
    var s3AccessKey = client.GetSecret("BeeStore-S3-AccessKey");
    var s3SecretKey = client.GetSecret("BeeStore-S3-SecretKey");
    var s3BucketName = client.GetSecret("BeeStore-BucketName");
    var s3BucketUrl = client.GetSecret("BeeStore-S3-BucketURL");
    var gatewayUrl = client.GetSecret("BeeStore-Gateway-URL");

    if (dbConnectionSecret.Value == null) throw new InvalidOperationException("DBConnection secret is missing on Key Vault.");
    if (s3AccessKey.Value == null) throw new InvalidOperationException("s3AccessKey secret is missing on Key Vault.");
    if (s3SecretKey.Value == null) throw new InvalidOperationException("s3SecretKey secret is missing on Key Vault.");
    if (s3BucketName.Value == null) throw new InvalidOperationException("s3BucketName secret is missing on Key Vault.");
    if (s3BucketUrl.Value == null) throw new InvalidOperationException("s3BucketUrl secret is missing on Key Vault.");
    if (gatewayUrl.Value == null) throw new InvalidOperationException("gatewayUrl secret is missing on Key Vault.");

    builder.Services.AddInfrastructuresService(dbConnectionSecret.Value.Value);

    builder.Services.AddJwtAuthenticationProduction(client);

    builder.Services.AddTransient<IPictureService>(provider =>
    {
        var unitOfWork = provider.GetRequiredService<UnitOfWork>();

        return new PictureService(
            new AmazonS3Client(s3AccessKey.Value.Value, s3SecretKey.Value.Value, RegionEndpoint.APNortheast1),
            s3BucketName.Value.Value,
            s3BucketUrl.Value.Value,
            unitOfWork
        );
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins",
            builder => builder.WithOrigins("http://localhost:3000", gatewayUrl.Value.Value)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials());
    });
}
builder.Services.AddValidatorsFromAssemblyContaining<BatchCreateDTOValidator>();


builder.Services.AddWebAPIService();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
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
    app.UseMiddleware<ApiKeyAuthMiddleware>();
    app.UseCors("AllowSpecificOrigins");
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
