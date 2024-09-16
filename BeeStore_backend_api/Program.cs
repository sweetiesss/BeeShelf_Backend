using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Api.Authentication;
using BeeStore_Repository.Data;
using BeeStore_Repository.Logger.GlobalExceptionHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySqlX.XDevAPI;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var configuration = builder.Configuration.Get<AppConfiguration>() ?? throw new ArgumentNullException("Configuration can not be configured.");
    builder.Services.AddInfrastructuresService(configuration.DatabaseConnection);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
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

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = client.GetSecret("BeeStore-JWT-Issuer").Value.Value ?? throw new ArgumentNullException("JWT-Issuer is not configured on Key Vault."),
            ValidAudience = client.GetSecret("BeeStore-JWT-Audience").Value.Value ?? throw new ArgumentNullException("JWT-Audience is not configured on Key Vault."),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(client.GetSecret("BeeStore-JWT-SecretKey").Value.Value
                                                        ?? throw new ArgumentNullException("BeeStore-JWT-SecretKey is not configured.")))
        };
    });
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
