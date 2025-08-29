using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using RestWithASPNET10Erudio.Configurations;
using Serilog;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Tools
{
    public class SqlServerFixture : IAsyncLifetime
    {
        public string ConnectionString { get; }

        public SqlServerFixture()
        {
            // Connection string provided by environment variable set in GitHub Actions
            ConnectionString = Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION_STRING")
                ?? "Server=sqlserver,1433;Database=TestDb;User Id=SA;Password=@Admin123;TrustServerCertificate=True";
        }

        public async Task InitializeAsync()
        {
            Log.Information("Using connection string: {ConnectionString}", ConnectionString);

            // Create TestDb database
            Log.Information("Creating TestDb database...");
            using var connection = new SqlConnection(ConnectionString.Replace("Database=TestDb", "Database=master"));
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

        public Task DisposeAsync()
        {
            Log.Information("SQL Server fixture disposed.");
            return Task.CompletedTask; // No container to dispose since managed by workflow
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