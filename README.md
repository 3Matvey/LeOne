

# Create migrations
dotnet ef migrations add InitialCreate --project LeOne.Infrastructure.Data --startup-project LeOne.API


# Run migrations
dotnet ef database update --project LeOne.Infrastructure.Data --startup-project LeOne.API --connection "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres"
