using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

[CollectionDefinition("E2E Database", DisableParallelization = true)]
public class E2EDatabaseCollection : ICollectionFixture<DbCleanupFixture> { }

public class DbCleanupFixture : IAsyncLifetime
{
    private readonly string _connectionString;

    private const string CommandText = @"
        DO $$
        DECLARE stmt text;
        BEGIN
          SELECT string_agg(format('%I.%I', schemaname, tablename), ', ')
            INTO stmt
          FROM pg_tables
          WHERE schemaname = 'leone'
            AND tablename <> '__EFMigrationsHistory';

          IF stmt IS NOT NULL THEN
            EXECUTE format('TRUNCATE TABLE %s RESTART IDENTITY CASCADE;', stmt);
          END IF;
        END $$;";

    public DbCleanupFixture()
    {
        var cfg = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        _connectionString = cfg.GetSection("LocalDbConnection")["leone"]
            ?? throw new InvalidOperationException("Connection string LocalDbConnection:leone not found in appsettings.Local.json");
    }

    public async Task InitializeAsync()
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = CommandText;
        cmd.CommandType = CommandType.Text;
        await cmd.ExecuteNonQueryAsync(); 
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
