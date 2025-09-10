using RestWithASPNET10Erudio.Mail.Settings;

namespace RestWithASPNET10Erudio.Configurations
{
    public static class EmailConfig
    {
        public static IServiceCollection AddEmailConfiguration(
            this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("Mail");
            var configs = section.Get<EmailSettings>();

            if (configs == null)
            {
                throw new ArgumentNullException(nameof(configs), "Mail configuration section not found.");
            }

            // Registramos como Singleton para ficar disponível via DI
            services.AddSingleton(configs);

            return services;
        }
    }
}
