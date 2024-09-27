using Amazon.S3;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.Data;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler;
using BeeStore_Repository.Mapper;
using BeeStore_Repository.Mapper.CustomResolver;
using BeeStore_Repository.Services;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace BeeStore_Repository
{
    public static class DenpendencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
        {
            // Scoped
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPictureService, PictureService>();
            services.AddScoped<IPartnerService, PartnerService>();
            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<UnitOfWork>();
            services.AddScoped<GlobalExceptionMiddleware>();
            services.AddScoped<CustomRoleNameResolver>();
            services.AddScoped<CustomRoleNameReverseResolver>();

            // Auto mapper
            services.AddAutoMapper(typeof(MapperConfigurationsProfile).Assembly);


            // Logger
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));


            // DB context
            services.AddDbContext<BeeStoreDbContext>(option => option.UseMySQL(databaseConnection));

            // Singleton
            services.AddSingleton<AppConfiguration>();
            services.AddSingleton<IAmazonS3, AmazonS3Client>();

            return services;


        }

        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.AddSingleton<GlobalExceptionMiddleware>();
            return services;
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
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
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            };
        });

        }

        public static void AddJwtAuthenticationProduction(this IServiceCollection services, SecretClient client)
        {
            services.AddAuthentication(options =>
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
    }
}