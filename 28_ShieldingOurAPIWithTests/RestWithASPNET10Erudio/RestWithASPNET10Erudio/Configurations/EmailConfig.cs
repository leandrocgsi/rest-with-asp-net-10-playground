using RestWithASPNET10Erudio.Mail.Settings;

namespace RestWithASPNET10Erudio.Configurations
{
    public static class EmailConfig
    {
        public static IServiceCollection AddEmailConfiguration(
            this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("Email");
            var configs = section.Get<EmailSettings>();

            if (configs == null)
            {
                throw new ArgumentNullException(
                    nameof(configs), "Email configuration section not found.");
            }
            // Carregar variáveis de ambiente para Username e Password
            configs.Username = Environment.GetEnvironmentVariable("EMAIL_USERNAME")
                ?? throw new ArgumentNullException("EMAIL_USERNAME", "Environment variable EMAIL_USERNAME not found.");
            
            configs.Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
                ?? throw new ArgumentNullException("EMAIL_PASSWORD", "Environment variable EMAIL_PASSWORD not found.");

            // Registramos como Singleton para ficar disponível via DI
            services.AddSingleton(configs);

            return services;
        }
    }
}
