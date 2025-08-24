/**
namespace RestWithASPNET10Erudio.Configurations
{
    public static class ApiVersioningConfig
    {
        public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                // Assume versão padrão se não for especificada
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // Reporta as versões suportadas e depreciadas nos headers
                options.ReportApiVersions = true;

                // Lê versão apenas do path (api/person/v1)
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            // Opcional: configurar API Explorer para Swagger
            /**
            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV"; // v1, v2
                setup.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}
*/