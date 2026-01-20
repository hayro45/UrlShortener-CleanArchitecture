using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Common.Behaviors;

namespace UrlShortener.Application;

/// <summary>
/// Extension methods for registering Application layer services.
/// Follows the composition root pattern for dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Register MediatR handlers from this assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(assembly);

        // Register the validation pipeline behavior
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
