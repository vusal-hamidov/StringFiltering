using Microsoft.Extensions.DependencyInjection;
using StringFiltering.Application.Interfaces;
using StringFiltering.Infrastructure.Background;
using StringFiltering.Infrastructure.Queue;
using StringFiltering.Infrastructure.Storage;

namespace StringFiltering.Infrastructure.CompositionRoot;

public static class InfrastructureCompositionRoot
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddSingleton<IFilteringQueue, ConcurrentFilteringQueue>()
            .AddSingleton<IChunkStorage, InMemoryChunkStorage>()
            .AddSingleton<IResultStore, InMemoryResultStore>()
            .AddHostedService<FilteringBackgroundService>();
        
        return services;
    }
}