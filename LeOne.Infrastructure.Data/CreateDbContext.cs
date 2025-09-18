using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LeOne.Infrastructure.Data
{
    public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(basePath, "appsettings.Testing.json"), optional: true, reloadOnChange: false);

            var config = builder.Build();

            var cs = config.GetConnectionString("LocalDbConnection");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(cs) 
                .Options;

            return new AppDbContext(options);
        }
    }
}
