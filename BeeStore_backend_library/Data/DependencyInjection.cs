using BeeStore_Repository.Mapper;
using BeeStore_Repository.Mapper.CustomResolver;
using BeeStore_Repository.Services;
using BeeStore_Repository.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace BeeStore_Repository.Data
{
    public static class DenpendencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
        {
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<UnitOfWork>();

            services.AddScoped<CustomRoleNameResolver>();
            //services.AddScoped<CustomRoleCodeResolver>();
            //services.AddScoped<CustomEmailResolver>();
            //services.AddScoped<CustomSubscriptionNameResolver>();
            services.AddAutoMapper(typeof(MapperConfigurationsProfile).Assembly);
            services.AddDbContext<BeeStoreDbContext>(option => option.UseMySQL(databaseConnection));

            return services;


        }

        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            return services;
        }


    }
}