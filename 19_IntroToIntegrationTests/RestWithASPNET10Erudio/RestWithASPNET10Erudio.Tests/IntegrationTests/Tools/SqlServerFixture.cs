using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using RestWithASPNET10Erudio.Configurations;
using Serilog;
using System;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Tools
{
    public class SqlServerFixture : IAsyncLifetime
    {
        public string ConnectionString { get; }

        public SqlServerFixture()
        {
            // Connection string provided by environment variable or fallback
            ConnectionString = Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION_STRING")
                ?? "Server=sqlserver,1433;Database=TestDb;User Id=SA;Password=@Admin123;TrustServerCertificate=True";
            Log.Information("SqlServerFixture initialized with connection string: {ConnectionString}", ConnectionString);
        }

        public async Task InitializeAsync()
        {
            try
            {
                Log.Information("Starting SQL Server fixture initialization...");

                // Create TestDb database
                Log.Information("Creating TestDb database...");
                using var connection = new SqlConnection(ConnectionString.Replace("Database=TestDb", "Database=master"));
                try
                {
                    await connection.OpenAsync();
                    using var command = new SqlCommand(
                        "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TestDb') CREATE DATABASE TestDb;",
                        connection);
                    await command.ExecuteNonQueryAsync();
                    Log.Information("TestDb database created or already exists.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to create TestDb database.");
                    throw;
                }
                finally
                {
                    connection.Close();
                }

                // Retry migrations
                await RetryPolicy.ExecuteAsync(async () =>
                {
                    Log.Information("Applying migrations to {Database}...", "TestDb");
                    try
                    {
                        EvolveConfig.ExecuteMigrations(ConnectionString);
                        Log.Information("Migrations applied successfully.");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to apply migrations.");
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize SQL Server fixture.");
                throw;
            }
        }

        public Task DisposeAsync()
        {
            Log.Information("SQL Server fixture disposed.");
            return Task.CompletedTask; // No container to dispose since managed by workflow
        }

        private static readonly AsyncRetryPolicy RetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(7, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                (exception, timeSpan, retryCount, _) =>
                {
                    Log.Warning("Retry {RetryCount} after {TimeSpan} seconds due to: {Exception}", retryCount, timeSpan.TotalSeconds, exception.Message);
                });
    }
}