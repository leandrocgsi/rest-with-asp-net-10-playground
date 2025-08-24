using Microsoft.OpenApi.Models;

namespace RestWithASPNET10Erudio.Configurations
{
    public static class SwaggerSpecification
    {
        public static IServiceCollection AddSwaggerSpecification(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var openApiInfo = provider.GetRequiredService<OpenApiInfo>();

            // Registra SwaggerGen com os dados do OpenAPI
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API V1", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "API V2", Version = "v2" });
                
                c.CustomSchemaIds(type =>
                {
                    // inclui namespace e nome da classe
                    return type.FullName.Replace("+", ".");
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerSpecification(this IApplicationBuilder app)
        {
            // JSON do Swagger em /swagger/v1/swagger.json
            app.UseSwagger();

            // UI em /swagger-ui
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sua API V1");
                c.RoutePrefix = "swagger-ui";
            });

            return app;
        }
    }
}