using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Coucher.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoucherSharedServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;

        return services;
    }
}
