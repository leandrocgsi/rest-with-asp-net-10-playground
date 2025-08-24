/**
using Scalar.AspNetCore;

namespace RestWithASPNET10Erudio.Configurations
{
    public static class ScalarConfig
    {
        private static readonly string AppName =
            "ASP.NET 2026 REST API's from 0 to Azure and GCP with .NET 10, Docker and Kubernetes";

        private static readonly string AppDescription =
            $"REST API RESTful developed in course '{AppName}'";

        public static IServiceCollection AddScalarConfiguration(this IServiceCollection services)
        {
            // Não há configuração extra no serviço para Scalar
            return services;
        }

        public static WebApplication UseScalarConfiguration(this WebApplication app)
        {
            app.MapScalarApiReference("/scalar", options =>
            {
                options
                    .WithTitle(AppName)
                    //.WithDescription(AppDescription)
                    .WithOpenApiRoutePattern("/swagger/v1/swagger.json"); // <- JSON correto
            });

            return app;
        }
    }
}
*/