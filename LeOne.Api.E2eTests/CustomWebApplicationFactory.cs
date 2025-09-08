using LeOne.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeOne.API.E2ETests;

internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var toRemove = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                            d.ServiceType == typeof(AppDbContext))
                .ToList();

            foreach (var d in toRemove)
                services.Remove(d);

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();

            var cfg = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

            var connectionString = cfg.GetSection("LocalDbConnection")["leone"]
                ?? throw new InvalidOperationException("Connection string LocalDbConnection:leone not found in appsettings.Test.json");

            services.AddDbContext<AppDbContext>(o =>
                o.UseNpgsql(connectionString));
        });
    }
}
