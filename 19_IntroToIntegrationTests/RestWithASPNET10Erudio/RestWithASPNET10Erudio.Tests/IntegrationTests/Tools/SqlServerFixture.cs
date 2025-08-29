using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using RestWithASPNET10Erudio.Configurations;
using Serilog;
using Testcontainers.MsSql;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Tools
{
    public class SqlServerFixture : IAsyncLifetime
    {
        public MsSqlContainer Container { get; }

        public string ConnectionString => Container.GetConnectionString().Replace("Database=master", "Database=TestDb");

        public SqlServerFixture()
        {
            // Disable ResourceReaper to avoid ryuk initialization issues
            TestcontainersSettings.ResourceReaperEnabled = false;

            Container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("@Your_password123!")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithNetworkAliases("sqlserver")
                .WithNetwork("test-network")
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilCommandIsCompleted("/opt/mssql-tools18/bin/sqlcmd -C -S localhost -U SA -P '@Your_password123!' -Q 'SELECT 1'")
                    .UntilPortIsAvailable(1433))
                .Build();
        }

        public async Task InitializeAsync()
        {
            Log.Information("Starting SQL Server container...");
            await Container.StartAsync();
            Log.Information("SQL Server container started with connection string: {ConnectionString}", ConnectionString);

            // Create TestDb database
            Log.Information("Creating TestDb database...");
            using var connection = new SqlConnection(Container.GetConnectionString());
            await connection.OpenAsync();
            using var command = new SqlCommand("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TestDb') CREATE DATABASE TestDb;", connection);
            await command.ExecuteNonQueryAsync();
            Log.Information("TestDb database created or already exists.");

            // Retry migrations
            await RetryPolicy.ExecuteAsync(async () =>
            {
                Log.Information("Applying migrations...");
                EvolveConfig.ExecuteMigrations(ConnectionString);
                Log.Information("Migrations applied successfully.");
            });
        }

        public async Task DisposeAsync()
        {
            await Container.DisposeAsync();
            Log.Information("SQL Server container disposed.");
        }

        private static readonly AsyncRetryPolicy RetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                (exception, timeSpan, retryCount, _) =>
                {
                    Log.Warning("Retry {RetryCount} after {TimeSpan} due to: {Exception}", retryCount, timeSpan, exception.Message);
                });
    }
}