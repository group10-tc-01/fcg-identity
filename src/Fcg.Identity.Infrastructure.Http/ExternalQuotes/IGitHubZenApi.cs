using Refit;

namespace Fcg.Identity.Infrastructure.Http.ExternalQuotes;

public interface IGitHubZenApi
{
    [Get("/zen")]
    Task<string> GetZenAsync(CancellationToken cancellationToken);
}
