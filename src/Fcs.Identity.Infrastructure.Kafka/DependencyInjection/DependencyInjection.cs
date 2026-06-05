using System.Diagnostics.CodeAnalysis;
using Fcs.Identity.Application.Abstractions.Messaging;
using Fcs.Identity.Infrastructure.Kafka.Messaging;
using Fcs.Identity.Infrastructure.Kafka.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fcs.Identity.Infrastructure.Kafka.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddKafkaInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<KafkaSettings>()
            .Bind(configuration.GetRequiredSection(KafkaSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IMessagePublisher, KafkaMessagePublisher>();

        return services;
    }
}
