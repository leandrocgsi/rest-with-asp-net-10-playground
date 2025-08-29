using DotNet.Testcontainers.Builders;
using RestWithASPNET10Erudio.Configurations;
using Serilog;
using Polly.Retry;
using Testcontainers.MsSql;
using Polly;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Tools
{
    public class SqlServerFixture : IAsyncLifetime
    {
        public MsSqlContainer Container { get; }

        public string ConnectionString => Container.GetConnectionString().Replace("Database=master", "Database=TestDb");

        public SqlServerFixture()
        {
            Container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("@Your_password123!")
                .WithNetworkAliases("sqlserver")
                .WithNetwork("test-network")
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilCommandIsCompleted("/opt/mssql-tools18/bin/sqlcmd -C -S localhost -U SA -P '@Your_password123!' -Q 'SELECT 1'"))
                .Build();
        }

        public async Task InitializeAsync()
        {
            await Container.StartAsync();
            Log.Information("SQL Server container started with connection string: {ConnectionString}", ConnectionString);

            // Add a retry mechanism for migrations
            await RetryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    EvolveConfig.ExecuteMigrations(ConnectionString);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to apply migrations on attempt. Retrying...");
                    throw;
                }
            });
        }

        public async Task DisposeAsync()
        {
            await Container.DisposeAsync();
        }

        // Custom retry policy for migrations
        private static readonly AsyncRetryPolicy RetryPolicy = Polly.Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Exponential backoff: 2s, 4s, 8s, 16s, 32s
                (exception, timeSpan, retryCount, context) =>
                {
                    Log.Warning("Retry {RetryCount} after {TimeSpan} due to: {Exception}", retryCount, timeSpan, exception.Message);
                });
    }
}