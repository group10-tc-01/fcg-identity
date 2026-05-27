namespace Fcg.Identity.Infrastructure.Http.Settings;

public sealed class ExternalQuoteRetryOptions
{
    public int Attempts { get; init; } = 3;
    public int BaseDelayMilliseconds { get; init; } = 200;
}
