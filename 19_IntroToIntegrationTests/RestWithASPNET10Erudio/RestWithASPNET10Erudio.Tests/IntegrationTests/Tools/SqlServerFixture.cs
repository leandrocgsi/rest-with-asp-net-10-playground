using Testcontainers.MsSql;
using Microsoft.Data.SqlClient;
using EvolveDb;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Tools
{
    public class SqlServerFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _container;

        public string ConnectionString => _container.GetConnectionString();

        public SqlServerFixture()
        {
            _container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("@Admin123")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _container.StartAsync();

            // Espera ativa até o SQL responder (resiliência em CI)
            await RetryUntilSqlAvailable(ConnectionString);

            // Roda as migrations via Evolve
            using var conn = new SqlConnection(ConnectionString);
            var evolve = new Evolve(conn, msg => Console.WriteLine(msg))
            {
                Locations = new List<string> { "db/migrations", "db/dataset" },
                IsEraseDisabled = true
            };
            evolve.Migrate();
        }

        public async Task DisposeAsync() =>
            await _container.StopAsync();

        private async Task RetryUntilSqlAvailable(string connStr)
        {
            const int maxRetries = 10;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    using var conn = new SqlConnection(connStr);
                    await conn.OpenAsync();
                    return;
                }
                catch
                {
                    await Task.Delay(3000);
                }
            }
            throw new InvalidOperationException("SQL Server didn't start in time.");
        }
    }

    [CollectionDefinition("SQL Server Collection")]
    public class SqlServerCollection : ICollectionFixture<SqlServerFixture> { }
}
