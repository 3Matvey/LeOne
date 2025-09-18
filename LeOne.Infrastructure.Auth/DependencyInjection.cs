using LeOne.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LeOne.Infrastructure.Auth
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            var jwt = new JwtSettings();
            configuration.GetSection("Jwt").Bind(jwt);
            services.AddSingleton(Options.Create(jwt));

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            return services;
        }
    }
}