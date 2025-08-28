using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestWithASPNET10Erudio.Model.Context;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Tools
{
    internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString;

        public CustomWebApplicationFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development"); // força Evolve a rodar

            builder.ConfigureServices(services =>
            {
                // remove configuração original do contexto
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<MSSQLContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // registra contexto usando o banco do container
                services.AddDbContext<MSSQLContext>(options =>
                    options.UseSqlServer(_connectionString));
            });
        }
    }
}