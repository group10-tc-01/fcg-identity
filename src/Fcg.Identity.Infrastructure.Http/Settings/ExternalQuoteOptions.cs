namespace Fcg.Identity.Infrastructure.Http.Settings;

public sealed class ExternalQuoteOptions
{
    public const string SectionName = "ExternalQuote";

    public string BaseUrl { get; init; } = "https://api.github.com";
    public int TimeoutSeconds { get; init; } = 10;
    public ExternalQuoteRetryOptions Retry { get; init; } = new();
}
