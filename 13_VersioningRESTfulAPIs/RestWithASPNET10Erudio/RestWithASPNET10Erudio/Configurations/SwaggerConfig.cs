using Microsoft.OpenApi.Models;

namespace RestWithASPNET10Erudio.Configurations
{
    public static class SwaggerSpecification
    {
        public static IServiceCollection AddSwaggerSpecification(this IServiceCollection services)
        {
            services.AddSwaggerGen();

            return services;
        }

        public static IApplicationBuilder UseSwaggerSpecification(this IApplicationBuilder app)
        {
            // Recupera OpenApiInfo do DI
            var openApiInfo = app.ApplicationServices.GetRequiredService<OpenApiInfo>();

            // Configura SwaggerGen com os dados do OpenApiSpecification
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", openApiInfo.Title);
                c.RoutePrefix = "swagger";
                c.DocumentTitle = openApiInfo.Title;
            });

            return app;
        }
    }
}
