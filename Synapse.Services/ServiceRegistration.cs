using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Synapse.Service.Tasking.HttpClients;
using Synapse.Service.Tasking.Services.Abstraction;
using Synapse.Service.Tasking.Services.Implement;
using Synapse.Services.Services.Abstraction;
using Synapse.Services.Services.Implement;
using Synapse.Shared.Options;

namespace Synapse.Services
{
    /// <summary>
    /// Register all services for Services layer
    /// </summary>
    public static class ServiceRegistration
    {
        public static IServiceCollection AddSynapseServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Get AppSettings
            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>() ?? new AppSettings();

            // Register Token Storage
            services.AddSingleton<ITokenStorage, FileTokenStorage>();

            // Register IdentityOptions for HTTP clients
            services.AddSingleton<IdentityOptions>(sp =>
            {
                var tokenStorage = sp.GetRequiredService<ITokenStorage>();
                var (accessToken, refreshToken) = Task.Run(async () => await tokenStorage.LoadTokensAsync()).GetAwaiter().GetResult();

                return new IdentityOptions
                {
                    Url = appSettings.ApiBaseUrl,
                    ApiGatewayUrl = appSettings.ApiGatewayUrl,
                    AccessToken = accessToken ?? string.Empty,
                    RefreshToken = refreshToken ?? string.Empty,
                    SaveToken = async (access, refresh) =>
                    {
                        await tokenStorage.SaveTokensAsync(access, refresh);
                    }
                };
            });

            // Register Authentication Service
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            // Register HTTP Clients
            services.AddSingleton<TaskingClsHttp>(sp =>
            {
                var identityOptions = sp.GetRequiredService<IdentityOptions>();
                return new TaskingClsHttp(identityOptions, identityOptions.ApiGatewayUrl);
            });
            services.AddSingleton<TaskingHttpObject>();

            // Register Business Services
            services.AddTransient<IUserService, UserService>();
            services.AddSingleton<IDashboardService, MockDashboardService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddTransient<ISequenceService, SequenceService>();

            return services;
        }
    }
}
