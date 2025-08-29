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
                .WithPassword("@Your_password123!") // senha obrigatória para SQL Server
                .Build();
        }

        public async Task InitializeAsync()
        {
            await Container.StartAsync();
            EvolveConfig.ExecuteMigrations(ConnectionString);
        }

        public async Task DisposeAsync()
        {
            await Container.DisposeAsync();
        }
    }
}
