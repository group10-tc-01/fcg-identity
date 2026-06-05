using System.Diagnostics;

namespace Fcs.Identity.WebApi.Middlewares;

public sealed class RequestFlowLoggingMiddleware
{
    private static readonly PathString[] IgnoredPathPrefixes =
    [
        new("/health"),
        new("/metrics"),
        new("/swagger")
    ];

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestFlowLoggingMiddleware> _logger;

    public RequestFlowLoggingMiddleware(RequestDelegate next, ILogger<RequestFlowLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldSkip(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path.Value;

        _logger.LogInformation(
            "HTTP request started {Method} {Path}.",
            method,
            path);

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            "HTTP request finished {Method} {Path} with status {StatusCode} in {ElapsedMs} ms.",
            method,
            path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }

    private static bool ShouldSkip(PathString path)
    {
        return IgnoredPathPrefixes.Any(path.StartsWithSegments);
    }
}
