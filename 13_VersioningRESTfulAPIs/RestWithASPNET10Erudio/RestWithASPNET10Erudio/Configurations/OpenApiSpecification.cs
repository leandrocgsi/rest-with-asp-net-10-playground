namespace RestWithASPNET10Erudio.Configurations
{
    public static class OpenApiSpecification
    {
        private static readonly string AppName =
            "ASP.NET 2026 REST API's from 0 to Azure and GCP with .NET 10, Docker and Kubernetes";

        private static readonly string AppDescription =
            $"REST API RESTful developed in course '{AppName}'";

        public static IServiceCollection AddOpenApiSpecification(this IServiceCollection services)
        {
            // Temos um erro ao tentar usar o OpenApiInfo CS0246
            services.AddSingleton(new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = AppName,
                Version = "v1",
                Description = AppDescription,
                // Temos um erro ao tentar usar o OpenApiContact CS0246

                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "Leandro Costa",
                    Email = "https://github.com/leandrocgsi"
                }
            });

            return services;
        }
    }
}
