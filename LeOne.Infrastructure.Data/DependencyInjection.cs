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

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ISpaServiceRepository, SpaServiceRepository>();

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            return services;
        }
    }
}
