using Microsoft.OpenApi.Models;

namespace RestWithASPNET10Erudio.Configurations
{
    public static class OpenApiSpecification
    {
        private static readonly string AppName =
            "ASP.NET 2026 REST API's from 0 to Azure and GCP with .NET 10, Docker e Kubernetes";

        private static readonly string AppDescription =
            $"REST API RESTful developed in course '{AppName}'";

        public static IServiceCollection AddOpenApiSpecification(this IServiceCollection services)
        {
            // Apenas dados que podem ser compartilhados
            services.AddSingleton(new OpenApiInfo
            {
                Title = AppName,
                Version = "v1",
                Description = AppDescription,
                Contact = new OpenApiContact
                {
                    Name = "Leandro Costa",
                    Url = new Uri("https://github.com/leandrocgsi")
                }
            });

            return services;
        }
    }
}
