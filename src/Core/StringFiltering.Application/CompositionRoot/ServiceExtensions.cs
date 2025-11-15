using Microsoft.Extensions.DependencyInjection;
using StringFiltering.Application.Features.Result.Services;
using StringFiltering.Application.Features.Upload.Services;

namespace StringFiltering.Application.CompositionRoot;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IUploadService, UploadService>()
            .AddScoped<IResultService, ResultService>();
        
        return services;
    }
}