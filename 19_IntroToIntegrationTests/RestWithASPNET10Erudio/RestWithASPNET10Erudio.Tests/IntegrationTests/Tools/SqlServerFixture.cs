using RestWithASPNET10Erudio.Configurations;
using Testcontainers.MsSql;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Tools
{
    public class SqlServerFixture : IAsyncLifetime
    {
        private readonly ILogger<SqlServerFixture> _logger;

        public MsSqlContainer Container { get; }

        public string ConnectionString => Container.GetConnectionString();

        public SqlServerFixture()
        {
            // 🔵 Cria um Logger simples
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            _logger = loggerFactory.CreateLogger<SqlServerFixture>();

            _logger.LogInformation("[SqlServerFixture] Construindo container SQL Server...");

            Container = new MsSqlBuilder()
                .WithPassword("@Your_password123!")
                .Build();

            _logger.LogInformation("[SqlServerFixture] Container construído.");
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("[SqlServerFixture] Iniciando container...");
            await Container.StartAsync();
            _logger.LogInformation("[SqlServerFixture] Container iniciado.");

            _logger.LogInformation("[SqlServerFixture] Aguardando SQL Server estar pronto...");
            var retries = 10;
            while (retries > 0)
            {
                try
                {
                    using var conn = new SqlConnection(ConnectionString);
                    await conn.OpenAsync();
                    _logger.LogInformation("[SqlServerFixture] Conexão com SQL Server estabelecida.");
                    break;
                }
                catch
                {
                    _logger.LogWarning("[SqlServerFixture] SQL Server ainda não está pronto, aguardando 5 segundos...");
                    await Task.Delay(5000);
                    retries--;
                }
            }

            _logger.LogInformation("[SqlServerFixture] Executando migrations com Evolve...");
            EvolveConfig.ExecuteMigrations(ConnectionString);
            _logger.LogInformation("[SqlServerFixture] Migrations concluídas.");
        }

        public async Task DisposeAsync()
        {
            _logger.LogInformation("[SqlServerFixture] Descartando container...");
            await Container.DisposeAsync();
            _logger.LogInformation("[SqlServerFixture] Container descartado.");
        }
    }
}