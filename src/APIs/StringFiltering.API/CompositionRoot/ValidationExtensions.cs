using System.Reflection;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace StringFiltering.API.CompositionRoot;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services
            .AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly()])
            .AddFluentValidationAutoValidation(x => x.DisableBuiltInModelValidation = true);
        
        return services;
    }
}