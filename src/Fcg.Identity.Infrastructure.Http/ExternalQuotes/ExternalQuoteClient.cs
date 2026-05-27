using Fcg.Identity.Application.Abstractions.ExternalServices;

namespace Fcg.Identity.Infrastructure.Http.ExternalQuotes;

public sealed class ExternalQuoteClient : IExternalQuoteClient
{
    private readonly IGitHubZenApi _api;

    public ExternalQuoteClient(IGitHubZenApi api)
    {
        _api = api;
    }

    public Task<string> GetZenAsync(CancellationToken cancellationToken) =>
        _api.GetZenAsync(cancellationToken);
}
