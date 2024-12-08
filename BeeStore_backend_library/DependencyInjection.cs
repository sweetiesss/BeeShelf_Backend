using Amazon.S3;
using Amazon.S3.Transfer;
using Azure.Security.KeyVault.Secrets;
using BeeStore_Repository.BackgroundServices;
using BeeStore_Repository.Data;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler;
using BeeStore_Repository.Mapper;
using BeeStore_Repository.Services;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using System.Text.Json.Serialization;

namespace BeeStore_Repository
{
    public static class DenpendencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
        {
            // Scoped
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPartnerService, PartnerService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<ILotService, LotService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IBatchService, BatchService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IWarehouseShipperService, WarehouseShipperService>();
            services.AddScoped<IWarehouseStaffService, WarehouseStaffService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITransferUtility, TransferUtility>();
            services.AddScoped<GlobalExceptionMiddleware>();

            //Background services
            services.AddHostedService<InventoryExpirationService>();

            //Memory Cache
            services.AddMemoryCache();

            // Auto mapper
            services.AddAutoMapper(typeof(MapperConfigurationsProfile).Assembly);

            // Logger
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));


            // DB context
            services.AddDbContext<BeeStoreDbContext>(option => option.UseLazyLoadingProxies().UseMySql(databaseConnection, ServerVersion.AutoDetect(databaseConnection)));


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
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddControllers()
                    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfiguration>();
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
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