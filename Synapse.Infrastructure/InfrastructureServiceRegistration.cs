using Microsoft.Extensions.DependencyInjection;
using Synapse.Infrastructure.Persistence;

namespace Synapse.Infrastructure
{
    /// <summary>
    /// Register all services for Infrastructure layer
    /// </summary>
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Ensure database is created
            CreateDatabase.EnsureDatabaseCreated();

            // Register Database Context (SQLite)
            services.AddDbContext<SynapseDbContext>();

            // Register DbContextFactory for safe multi-threaded operations
            services.AddDbContextFactory<SynapseDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
