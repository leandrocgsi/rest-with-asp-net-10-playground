using RestWithASPNET10Erudio.Hypermedia.Enricher;
using RestWithASPNET10Erudio.Hypermedia.Filters;

namespace RestWithASPNET10Erudio.Configurations
{
    public static class HATEOASConfig
    {
        public static IServiceCollection AddHATEOASConfiguration(this IServiceCollection services)
        {
            var filterOptions = new HyperMediaFilterOptions();
            filterOptions.ContentResponseEnricherList.Add(new PersonEnricher());
            filterOptions.ContentResponseEnricherList.Add(new BookEnricher());

            services.AddSingleton(filterOptions);

            // Registrando o filtro para ser resolvido via DI
            services.AddScoped<HyperMediaFilter>();

            return services;
        }

        public static void UseHATEOASRoutes(this IEndpointRouteBuilder app)
        {
            app.MapControllerRoute("DefaultApi", "{controller=values}/v1/{id?}");
        }
    }
}
