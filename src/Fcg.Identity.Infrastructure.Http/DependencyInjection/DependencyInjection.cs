using Fcg.Identity.Application.Abstractions.ExternalServices;
using Fcg.Identity.Infrastructure.Http.ExternalQuotes;
using Fcg.Identity.Infrastructure.Http.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Refit;

namespace Fcg.Identity.Infrastructure.Http.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddHttpInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ExternalQuoteOptions>(configuration.GetSection(ExternalQuoteOptions.SectionName));

        services.AddRefitClient<IGitHubZenApi>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var externalQuote = serviceProvider.GetRequiredService<IOptions<ExternalQuoteOptions>>().Value;
                client.BaseAddress = new Uri(externalQuote.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(externalQuote.TimeoutSeconds);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Fcg.Identity/1.0");
            })
            .AddPolicyHandler((serviceProvider, _) =>
            {
                var retry = serviceProvider.GetRequiredService<IOptions<ExternalQuoteOptions>>().Value.Retry;
                return CreateExternalQuoteRetryPolicy(retry);
            });

        services.AddScoped<IExternalQuoteClient, ExternalQuoteClient>();

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateExternalQuoteRetryPolicy(ExternalQuoteRetryOptions retry)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retry.Attempts,
                attempt => TimeSpan.FromMilliseconds(retry.BaseDelayMilliseconds * attempt));
    }
}
