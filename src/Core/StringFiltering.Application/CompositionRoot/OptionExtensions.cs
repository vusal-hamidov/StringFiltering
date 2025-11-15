using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StringFiltering.Application.Options;

namespace StringFiltering.Application.CompositionRoot;

public static class OptionExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FilteringOptions>(configuration.GetSection(FilteringOptions.SectionName));
        return services;
    }
}