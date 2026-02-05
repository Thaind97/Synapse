using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Synapse.Shared.Options;
using System.Diagnostics;
using System.IO;

namespace Synapse.Presentation.Configuration
{
    /// <summary>
    /// Helper class to load application settings using IOptions pattern
    /// </summary>
    public static class AppSettingsLoader
    {
        private const string ConfigFileName = "appsettings.json";

        /// <summary>
        /// Load IConfiguration from appsettings.json
        /// </summary>
        public static IConfiguration LoadConfiguration()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(ConfigFileName, optional: true, reloadOnChange: true);

            Debug.WriteLine($"Loading configuration from: {configPath}");

            return builder.Build();
        }

        /// <summary>
        /// Register AppSettings with IOptions pattern
        /// </summary>
        public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind AppSettings section to AppSettings class
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            // Also register AppSettings directly for simple injection
            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>() ?? new AppSettings();
            services.AddSingleton(appSettings);

            Debug.WriteLine($"AppSettings loaded - LoginMode: {appSettings.LoginMode}");

            return services;
        }

        /// <summary>
        /// Load AppSettings directly (for backward compatibility)
        /// </summary>
        public static AppSettings Load()
        {
            var configuration = LoadConfiguration();
            return configuration.GetSection("AppSettings").Get<AppSettings>() ?? new AppSettings();
        }
    }
}
