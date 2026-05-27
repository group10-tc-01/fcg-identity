using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Infrastructure.Kafka.Messaging;
using Fcg.Identity.Infrastructure.Kafka.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Identity.Infrastructure.Kafka.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddKafkaInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection(KafkaSettings.SectionName));
        services.AddSingleton<IMessagePublisher, KafkaMessagePublisher>();
        return services;
    }
}
