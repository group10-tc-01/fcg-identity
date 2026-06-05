using Fcs.Identity.Infrastructure.Keycloak.Http.Contracts;
using Refit;

namespace Fcs.Identity.Infrastructure.Keycloak.Http;

public interface IKeycloakApi
{
    [Post("/realms/{realm}/protocol/openid-connect/token")]
    Task<ApiResponse<KeycloakTokenResponse>> GetTokenAsync(
        string realm,
        [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> form,
        CancellationToken cancellationToken);

    [Post("/admin/realms/{realm}/users")]
    Task<IApiResponse> CreateUserAsync(
        string realm,
        [Header("Authorization")] string authorization,
        [Body] CreateKeycloakUserRequest request,
        CancellationToken cancellationToken);

    [Get("/admin/realms/{realm}/users")]
    Task<ApiResponse<List<KeycloakUserResponse>>> FindUsersAsync(
        string realm,
        [Header("Authorization")] string authorization,
        [Query] string email,
        [Query] bool exact,
        CancellationToken cancellationToken);

    [Delete("/admin/realms/{realm}/users/{userId}")]
    Task<IApiResponse> DeleteUserAsync(
        string realm,
        string userId,
        [Header("Authorization")] string authorization,
        CancellationToken cancellationToken);

    [Put("/admin/realms/{realm}/users/{userId}/reset-password")]
    Task<IApiResponse> ResetPasswordAsync(
        string realm,
        string userId,
        [Header("Authorization")] string authorization,
        [Body] KeycloakCredential credential,
        CancellationToken cancellationToken);

    [Get("/admin/realms/{realm}/roles/{roleName}")]
    Task<ApiResponse<KeycloakRoleResponse>> GetRealmRoleAsync(
        string realm,
        string roleName,
        [Header("Authorization")] string authorization,
        CancellationToken cancellationToken);

    [Post("/admin/realms/{realm}/users/{userId}/role-mappings/realm")]
    Task<IApiResponse> AssignRealmRolesAsync(
        string realm,
        string userId,
        [Header("Authorization")] string authorization,
        [Body] IReadOnlyCollection<KeycloakRoleResponse> roles,
        CancellationToken cancellationToken);
}
