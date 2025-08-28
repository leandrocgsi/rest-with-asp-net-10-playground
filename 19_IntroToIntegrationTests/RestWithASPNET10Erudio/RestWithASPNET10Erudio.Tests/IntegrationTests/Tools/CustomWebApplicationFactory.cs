using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RestWithASPNET10Erudio.Configurations;

namespace RestWithASPNET10Erudio.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _connectionString;

        public CustomWebApplicationFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var dict = new Dictionary<string, string?>
                {
                    {
                        "MSSQLServerSQLConnection:MSSQLServerSQLConnectionString",
                        _connectionString
                    }
                };
                config.AddInMemoryCollection(dict!);
            });

            builder.ConfigureServices(services =>
            {
                // Executa as migrations no banco de teste (Testcontainers)
                EvolveConfig.ExecuteMigrations(_connectionString);
            });
        }
    }
}
