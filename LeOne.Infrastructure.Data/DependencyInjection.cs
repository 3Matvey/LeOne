using LeOne.Application.Common.Interfaces;
using LeOne.Domain.Entities;
using LeOne.Domain.Repositories;
using LeOne.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeOne.Infrastructure.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("LeOneDatabase"))
            );

            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<Product>, ProductRepository>();
            services.AddScoped<IRepository<Review>, ReviewRepository>();
            services.AddScoped<IRepository<SpaService>, SpaServiceRepository>();

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            return services;
        }
    }
}
