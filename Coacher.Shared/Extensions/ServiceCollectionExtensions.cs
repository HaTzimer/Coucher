using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Coacher.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoacherSharedServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;

        return services;
    }
}
