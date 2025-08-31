namespace RestWithASPNET10Erudio.Configurations
{
    public static class CorsConfig
    {
        private static string[] GetAllowedOrigins
            (IConfiguration configuration) =>  configuration
                .GetSection("Cors:Origins")
                .Get<string[]>()
                    ?? Array.Empty<string>();
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            string[] origins = GetAllowedOrigins(configuration);

            services.AddCors(options =>
            {
                /*
                options.AddPolicy("LocalPolicy",
                    policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());

                options.AddPolicy("MultipleOriginsPolicy",
                    policy => policy.WithOrigins(
                            "http://localhost:3000",
                            "http://localhost:8080",
                            "https://erudio.com.br")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());

                */
                options.AddPolicy("DefaultPolicy", builder =>
                {
                    builder.WithOrigins(origins)
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });

            return services;
        }



        public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app, IConfiguration configuration)
        // public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app)
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

            // app.UseCors();
            app.UseCors("DefaultPolicy");

            return app;
        }
    }
}