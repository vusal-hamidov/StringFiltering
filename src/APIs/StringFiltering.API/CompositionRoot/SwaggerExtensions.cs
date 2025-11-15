namespace StringFiltering.API.CompositionRoot;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
        
        return services;
    }
    
    public static void UseSwagger(this WebApplication app, IConfiguration configuration)
    {
        app
            .UseSwagger()
            .UseSwaggerUI();
    }
}