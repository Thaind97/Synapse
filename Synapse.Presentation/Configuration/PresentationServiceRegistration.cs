using Microsoft.Extensions.DependencyInjection;
using Synapse.Presentation.Services;
using Synapse.Presentation.ViewModels;

namespace Synapse.Presentation.Configuration
{
    /// <summary>
    /// Register all services for Presentation layer
    /// </summary>
    public static class PresentationServiceRegistration
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            // Register Localization Service
            services.AddSingleton<ILocalizationService, LocalizationService>();

            // Register ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<EVControlViewModel>();
            services.AddTransient<SequenceEditorViewModel>();
            services.AddTransient<ManageBatteriesViewModel>();

            return services;
        }
    }
}
