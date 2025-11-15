using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StringFiltering.Application.Interfaces;
using StringFiltering.Application.Services;
using StringFiltering.Domain.Interfaces;
using StringFiltering.Domain.Strategies;

namespace StringFiltering.Application.CompositionRoot;

public static class ApplicationCompositionRoot
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions(configuration)
            .AddServices()
            .AddSingleton<ITextFilter, TextFilter>()
            .AddSingleton<ITextFilterStrategy, JaroWinklerStrategy>()
            .AddSingleton<ITextFilterStrategy, LevenshteinStrategy>();
        
        return services;
    }
}