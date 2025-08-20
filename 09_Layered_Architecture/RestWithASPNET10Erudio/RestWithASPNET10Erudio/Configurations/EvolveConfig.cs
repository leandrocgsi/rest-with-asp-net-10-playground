using EvolveDb;
using Microsoft.Data.SqlClient;
using Serilog;

namespace RestWithASPNETErudio.Configurations
{
    public static class EvolveConfig
    {
        public static IServiceCollection AddEvolveMigrations(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                var connectionString = configuration[
                    "MSSQLServerSQLConnection:MSSQLServerSQLConnectionString"];

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException(
                        "Connection string 'MSSQLServerSQLConnectionString' not found.");
                }

                try
                {
                    using var evolveConnection = new SqlConnection(connectionString);

                    var evolve = new Evolve(evolveConnection, msg => Log.Information(msg))
                    {
                        Locations = new List<string> { "db/migrations", "db/dataset" },
                        IsEraseDisabled = true
                    };

                    evolve.Migrate();
                }
                catch (Exception ex)
                {
                    Log.Error("Database migration failed", ex);
                    throw;
                }
            }

            return services;
        }
    }
}
