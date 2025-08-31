namespace RestWithASPNET10Erudio.Configurations
{
    public static class CorsConfig
    {
        // Recupera as origens do appsettings.json uma única vez
        private static string[] GetAllowedOrigins(IConfiguration configuration) =>
            configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();

        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = GetAllowedOrigins(configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("LocalPolicy",
                    policy => policy.WithOrigins(allowedOrigins.Where(o => o.Contains("localhost")).ToArray())
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials());

                options.AddPolicy("MultipleOriginsPolicy",
                    policy => policy.WithOrigins(allowedOrigins)
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials());

                options.AddPolicy("DefaultPolicy", builder =>
                {
                     builder.WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                });
            });

            return services;
        }

        public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app, IConfiguration configuration)
        {
            var allowedOrigins = GetAllowedOrigins(configuration);

            // Middleware rigoroso para bloquear qualquer origem não permitida
            app.Use(async (context, next) =>
            {
                var origin = context.Request.Headers["Origin"].FirstOrDefault();

                if (!string.IsNullOrEmpty(origin) && !allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("CORS origin not allowed.");
                    return;
                }

                await next();
            });

            // Aplica a policy padrão do ASP.NET Core para adicionar headers
            app.UseCors();

            return app;
        }
    }
}