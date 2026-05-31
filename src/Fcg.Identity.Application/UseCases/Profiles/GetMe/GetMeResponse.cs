namespace Fcg.Identity.Application.UseCases.Profiles.GetMe;

public sealed record GetMeResponse(
    Guid Id,
    string KeycloakUserId,
    string FullName,
    string Email,
    string Role);
