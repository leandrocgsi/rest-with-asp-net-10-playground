using DotNet.Testcontainers.Builders;
using Microsoft.Data.SqlClient;
using RestWithASPNET10Erudio.Configurations;
using Testcontainers.MsSql;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Tools
{
    public class SqlServerFixture : IAsyncLifetime
    {
        public MsSqlContainer Container { get; }

        public string ConnectionString => Container.GetConnectionString();

        public SqlServerFixture()
        {
            Container = new MsSqlBuilder()
                .WithPassword("@Admin123")
                .WithWaitStrategy(
                    Wait.ForUnixContainer()
                        .UntilMessageIsLogged(
                        "SQL Server is now ready for client connections"))
                .Build();
        }

        public async Task InitializeAsync()
        {
            await Container.StartAsync();

            // ALTERADO: garantir que a conexão realmente esteja pronta antes do Evolve
            var retries = 10;
            while (retries > 0)
            {
                try
                {
                    using var connection = new SqlConnection(ConnectionString);
                    await connection.OpenAsync();
                    break; // conexão ok
                }
                catch
                {
                    retries--;
                    await Task.Delay(3000); // espera 3s e tenta de novo
                }
            }

            EvolveConfig.ExecuteMigrations(ConnectionString);
        }

        public async Task DisposeAsync()
        {
            await Container.DisposeAsync();
        }

    }
}
