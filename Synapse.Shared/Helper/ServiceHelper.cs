using Microsoft.Extensions.DependencyInjection;
using System;

namespace Synapse.Shared.Helper
{
    public static class ServiceHelper
    {
        public static IServiceProvider? ServiceProvider { get; set; }

        public static T? Resolve<T>()
        {
            return ServiceProvider != null ? ServiceProvider.GetService<T>() : default;
        }

        public static T? GetService<T>()
        {
            return ServiceProvider != null ? ServiceProvider.GetService<T>() : default;
        }

        public static T GetRequiredService<T>() where T : notnull
        {
            if (ServiceProvider == null)
                throw new InvalidOperationException("ServiceProvider is not initialized");
            
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}
