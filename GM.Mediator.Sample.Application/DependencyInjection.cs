using System.Reflection;
using FluentValidation;
using GM.Mediator.Contracts;
using GM.Mediator.Sample.Application.Behaviours;
using Microsoft.Extensions.DependencyInjection;

namespace GM.Mediator.Sample.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the application layer: the mediator and its handlers, the FluentValidation
    /// validators, and the request pipeline behaviours (validation + performance logging).
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddGMMediator(assembly);
        services.AddValidatorsFromAssembly(assembly);

        // Behaviours run in registration order: validate first, then measure the handler.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));

        return services;
    }
}
