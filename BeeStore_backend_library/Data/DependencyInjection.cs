using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler;
using BeeStore_Repository.Mapper;
using BeeStore_Repository.Mapper.CustomResolver;
using BeeStore_Repository.Services;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Configuration;
using System.Diagnostics;

namespace BeeStore_Repository.Data
{
    public static class DenpendencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<UnitOfWork>();
            services.AddScoped<GlobalExceptionMiddleware>();
            services.AddScoped<CustomRoleNameResolver>();
            services.AddAutoMapper(typeof(MapperConfigurationsProfile).Assembly);
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(),"/nlog.config"));



            services.AddDbContext<BeeStoreDbContext>(option => option.UseMySQL(databaseConnection));
            services.AddSingleton<AppConfiguration>();
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


    }
}