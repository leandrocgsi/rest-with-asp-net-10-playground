using RestWithASPNET10Erudio.Configurations;
using Testcontainers.MsSql;
using System;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Tools
{
    public class SqlServerFixture : IAsyncLifetime
    {
        private readonly bool _isCi;
        private MsSqlContainer? _container;

        public string ConnectionString { get; private set; } = string.Empty;

        public SqlServerFixture()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _isCi = string.Equals(env, "CONTINUOUS_DEPLOYMENT_ENV", StringComparison.OrdinalIgnoreCase);

            if (_isCi)
            {
                // CI/CD → conecta no SQL Server já iniciado no workflow
                ConnectionString = "Data Source=localhost,1433;" +
                                   "Initial Catalog=asp_net_10_erudio;" +
                                   "User Id=sa;" +
                                   "Password=@Admin123;" +
                                   "TrustServerCertificate=True;";
            }
            else
            {
                // Dev → container ainda não iniciado
                _container = new MsSqlBuilder()
                    .WithPassword("@Your_password123!")
                    .Build();
            }
        }

        public async Task InitializeAsync()
        {
            if (!_isCi && _container != null)
            {
                await _container.StartAsync();
                ConnectionString = _container.GetConnectionString();
            }

            // sempre roda migrations (CI e Dev)
            EvolveConfig.ExecuteMigrations(ConnectionString);
        }

        public async Task DisposeAsync()
        {
            if (!_isCi && _container != null)
                await _container.DisposeAsync();
        }
    }
}
