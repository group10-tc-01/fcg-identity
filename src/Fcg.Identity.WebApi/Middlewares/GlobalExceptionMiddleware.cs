using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Fcg.Identity.WebApi.Models;

namespace Fcg.Identity.WebApi.Middlewares;

[ExcludeFromCodeCoverage]
public sealed class GlobalExceptionMiddleware
{
    private const string CorrelationIdKey = "CorrelationId";
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}. Returning {StatusCode}. CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path.Value,
                StatusCodes.Status500InternalServerError,
                context.Items[CorrelationIdKey]);

            await WriteResponse(context, HttpStatusCode.InternalServerError, exception.Message);
        }
    }

    private static Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var payload = JsonSerializer.Serialize(ApiResponse<string>.FromFailure(message));
        return context.Response.WriteAsync(payload);
    }
}
