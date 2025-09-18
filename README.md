# Create migrations
dotnet ef migrations add MigrationName --project LeOne.Infrastructure.Data


# Run migrations
dotnet ef database update --project LeOne.Infrastructure.Data --startup-project LeOne.API --connection "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres"
