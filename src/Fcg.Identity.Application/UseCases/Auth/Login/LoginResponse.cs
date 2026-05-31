namespace Fcg.Identity.Application.UseCases.Auth.Login;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType);
