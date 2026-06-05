using System.Security.Claims;
using System.Text.Json;
using Fcs.Identity.Application.Abstractions.Authentication;

namespace Fcs.Identity.WebApi.Authentication;

public sealed class HttpContextCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => User.Identity?.IsAuthenticated == true;

    public string? KeycloakUserId =>
        User.FindFirstValue("sub") ??
        User.FindFirstValue(ClaimTypes.NameIdentifier);

    public IReadOnlyCollection<string> Roles => GetRoles().ToArray();

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    private IEnumerable<string> GetRoles()
    {
        foreach (var role in User.FindAll(ClaimTypes.Role).Select(claim => claim.Value))
        {
            yield return role;
        }

        foreach (var role in User.FindAll("role").Select(claim => claim.Value))
        {
            yield return role;
        }

        foreach (var role in User.FindAll("roles").Select(claim => claim.Value))
        {
            yield return role;
        }

        var realmAccess = User.FindFirstValue("realm_access");
        if (string.IsNullOrWhiteSpace(realmAccess))
        {
            yield break;
        }

        using var document = JsonDocument.Parse(realmAccess);
        if (!document.RootElement.TryGetProperty("roles", out var roles) || roles.ValueKind != JsonValueKind.Array)
        {
            yield break;
        }

        foreach (var role in roles.EnumerateArray())
        {
            if (role.ValueKind == JsonValueKind.String)
            {
                yield return role.GetString()!;
            }
        }
    }
}
