using System.Net;
using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Infrastructure.Keycloak.Http;
using Fcg.Identity.Infrastructure.Keycloak.Identity;
using Fcg.Identity.Infrastructure.Keycloak.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Refit;

namespace Fcg.Identity.Infrastructure.Keycloak.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddKeycloakInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKeycloakOptions(configuration);
        services.AddKeycloakHttpClient();
        services.AddKeycloakServices();

        return services;
    }

    private static IServiceCollection AddKeycloakOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<KeycloakSettings>()
            .Bind(configuration.GetRequiredSection(KeycloakSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddKeycloakHttpClient(this IServiceCollection services)
    {
        services.AddRefitClient<IKeycloakApi>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<KeycloakSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            })
            .AddPolicyHandler((serviceProvider, _) =>
            {
                var retry = serviceProvider.GetRequiredService<IOptions<KeycloakSettings>>().Value.Retry;
                return CreateKeycloakRetryPolicy(retry);
            });

        return services;
    }

    private static IServiceCollection AddKeycloakServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityProvider, KeycloakIdentityProvider>();

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateKeycloakRetryPolicy(KeycloakRetrySettings retry)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retry.RetryCount,
                attempt => TimeSpan.FromMilliseconds(retry.BaseDelayMilliseconds * Math.Pow(2, attempt - 1)));
    }
}
