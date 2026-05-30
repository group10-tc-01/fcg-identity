using System.Diagnostics.CodeAnalysis;

namespace Fcg.Identity.WebApi.Models;

[ExcludeFromCodeCoverage]
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }

    public static ApiResponse<T> FromSuccess(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static ApiResponse<T> FromFailure(string message) => new()
    {
        Success = false,
        Message = message
    };
}
