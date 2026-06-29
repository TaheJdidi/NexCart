using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexCart.Persistence.Extensions;

namespace NexCart.Infrastructure.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register persistence (DbContext) and other infrastructure services here
            services.AddPersistence(configuration);

            return services;
        }
    }
}
