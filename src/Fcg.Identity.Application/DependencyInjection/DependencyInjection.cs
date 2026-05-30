using System.Reflection;
using Fcg.Identity.Application.Abstractions.Behaviors;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Application.Messaging;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Identity.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<IMessagePublisher, NullMessagePublisher>();

        return services;
    }
}
