namespace Fcg.Identity.Application.Abstractions.ExternalServices;

public interface IExternalQuoteClient
{
    Task<string> GetZenAsync(CancellationToken cancellationToken);
}
