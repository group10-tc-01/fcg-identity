namespace Fcg.Identity.WebApi.Models;

public sealed record AuthSessionResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType);
