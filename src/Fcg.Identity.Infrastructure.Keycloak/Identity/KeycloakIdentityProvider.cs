using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Domain.Shared;
using Fcg.Identity.Domain.Shared.Results;
using Fcg.Identity.Infrastructure.Keycloak.Settings;
using Microsoft.Extensions.Options;

namespace Fcg.Identity.Infrastructure.Keycloak.Identity;

public sealed class KeycloakIdentityProvider : IIdentityProvider
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly KeycloakSettings _settings;

    public KeycloakIdentityProvider(HttpClient httpClient, IOptions<KeycloakSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<Result<CreateDonorIdentityUserResponse>> CreateDonorAsync(
        CreateDonorIdentityUserRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accessTokenResult = await GetAdminAccessTokenAsync(cancellationToken);
            if (accessTokenResult.IsFailure)
            {
                return accessTokenResult.Error;
            }

            using var createUserRequest = new HttpRequestMessage(HttpMethod.Post, BuildUri($"admin/realms/{_settings.Realm}/users"));
            createUserRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResult.Value);
            createUserRequest.Content = JsonContent.Create(CreateUserRepresentation(request), options: JsonSerializerOptions);

            using var createUserResponse = await _httpClient.SendAsync(createUserRequest, cancellationToken);

            if (createUserResponse.StatusCode == HttpStatusCode.Conflict)
            {
                return Error.Conflict("IdentityProvider.UserAlreadyExists", "A user with this email already exists in the identity provider.");
            }

            if (!createUserResponse.IsSuccessStatusCode)
            {
                return Error.Failure("IdentityProvider.CreateUserFailed", "Could not create donor user in the identity provider.");
            }

            var keycloakUserId = GetCreatedUserId(createUserResponse);
            if (string.IsNullOrWhiteSpace(keycloakUserId))
            {
                return Error.Failure("IdentityProvider.UserIdMissing", "The identity provider did not return the created user id.");
            }

            var assignRoleResult = await AssignRealmRoleAsync(keycloakUserId, IdentityRoles.Donor, accessTokenResult.Value, cancellationToken);
            if (assignRoleResult.IsFailure)
            {
                await DeleteUserAsync(keycloakUserId, accessTokenResult.Value, cancellationToken);
                return assignRoleResult.Error;
            }

            return new CreateDonorIdentityUserResponse(keycloakUserId);
        }
        catch (HttpRequestException)
        {
            return Error.Failure("IdentityProvider.Unavailable", "The identity provider is unavailable.");
        }
        catch (TaskCanceledException)
        {
            return Error.Failure("IdentityProvider.Timeout", "The identity provider request timed out.");
        }
    }

    public async Task<Result<LoginIdentityUserResponse>> LoginAsync(
        LoginIdentityUserRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenRequest = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = _settings.ClientId,
                ["username"] = request.Email,
                ["password"] = request.Password
            };

            using var response = await _httpClient.PostAsync(
                BuildUri($"realms/{_settings.Realm}/protocol/openid-connect/token"),
                new FormUrlEncodedContent(tokenRequest),
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Error.Unauthorized("IdentityProvider.InvalidCredentials", "Invalid email or password.");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return Error.Failure("IdentityProvider.LoginFailed", "Could not authenticate with the identity provider.");
            }

            if (!response.IsSuccessStatusCode)
            {
                return Error.Failure("IdentityProvider.LoginFailed", "Could not authenticate with the identity provider.");
            }

            var token = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(JsonSerializerOptions, cancellationToken);
            if (token is null || string.IsNullOrWhiteSpace(token.AccessToken))
            {
                return Error.Failure("IdentityProvider.TokenMissing", "The identity provider did not return an access token.");
            }

            return new LoginIdentityUserResponse(
                token.AccessToken,
                token.RefreshToken ?? string.Empty,
                token.ExpiresIn,
                token.TokenType);
        }
        catch (HttpRequestException)
        {
            return Error.Failure("IdentityProvider.Unavailable", "The identity provider is unavailable.");
        }
        catch (TaskCanceledException)
        {
            return Error.Failure("IdentityProvider.Timeout", "The identity provider request timed out.");
        }
    }

    private async Task<Result<string>> GetAdminAccessTokenAsync(CancellationToken cancellationToken)
    {
        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _settings.AdminClientId,
            ["username"] = _settings.AdminUsername,
            ["password"] = _settings.AdminPassword
        };

        using var response = await _httpClient.PostAsync(
            BuildUri($"realms/{_settings.AdminRealm}/protocol/openid-connect/token"),
            new FormUrlEncodedContent(tokenRequest),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure("IdentityProvider.AdminAuthenticationFailed", "Could not authenticate with the identity provider admin API.");
        }

        var token = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(JsonSerializerOptions, cancellationToken);
        if (string.IsNullOrWhiteSpace(token?.AccessToken))
        {
            return Error.Failure("IdentityProvider.AdminTokenMissing", "The identity provider did not return an admin access token.");
        }

        return token.AccessToken;
    }

    private async Task<Result<RoleRepresentation>> GetRealmRoleAsync(
        string roleName,
        string accessToken,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, BuildUri($"admin/realms/{_settings.Realm}/roles/{roleName}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Error.Failure("IdentityProvider.RoleNotFound", $"The role '{roleName}' was not found in the identity provider.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure("IdentityProvider.GetRoleFailed", "Could not resolve the donor role in the identity provider.");
        }

        var role = await response.Content.ReadFromJsonAsync<RoleRepresentation>(JsonSerializerOptions, cancellationToken);
        if (role is null || string.IsNullOrWhiteSpace(role.Name))
        {
            return Error.Failure("IdentityProvider.RoleInvalid", "The identity provider returned an invalid donor role.");
        }

        return role;
    }

    private async Task<Result<string>> AssignRealmRoleAsync(
        string keycloakUserId,
        string roleName,
        string accessToken,
        CancellationToken cancellationToken)
    {
        var roleResult = await GetRealmRoleAsync(roleName, accessToken, cancellationToken);
        if (roleResult.IsFailure)
        {
            return roleResult.Error;
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            BuildUri($"admin/realms/{_settings.Realm}/users/{keycloakUserId}/role-mappings/realm"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = JsonContent.Create(new[] { roleResult.Value }, options: JsonSerializerOptions);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure("IdentityProvider.AssignRoleFailed", "Could not assign the donor role in the identity provider.");
        }

        return keycloakUserId;
    }

    private async Task DeleteUserAsync(string keycloakUserId, string accessToken, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, BuildUri($"admin/realms/{_settings.Realm}/users/{keycloakUserId}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var _ = await _httpClient.SendAsync(request, cancellationToken);
    }

    private Uri BuildUri(string relativePath)
    {
        var baseUrl = _settings.BaseUrl.TrimEnd('/') + "/";
        return new Uri(new Uri(baseUrl), relativePath);
    }

    private static KeycloakUserRepresentation CreateUserRepresentation(CreateDonorIdentityUserRequest request)
    {
        return new KeycloakUserRepresentation(
            request.Email,
            request.Email,
            request.FullName,
            string.Empty,
            Enabled: true,
            EmailVerified: true,
            Credentials:
            [
                new KeycloakCredentialRepresentation("password", request.Password, Temporary: false)
            ],
            RequiredActions: []);
    }

    private static string? GetCreatedUserId(HttpResponseMessage response)
    {
        return response.Headers.Location?.Segments.LastOrDefault()?.TrimEnd('/');
    }

    private sealed record KeycloakTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string? RefreshToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn,
        [property: JsonPropertyName("token_type")] string TokenType);

    private sealed record KeycloakErrorResponse(
        [property: JsonPropertyName("error")] string? Error,
        [property: JsonPropertyName("error_description")] string? ErrorDescription);

    private sealed record KeycloakUserRepresentation(
        string Username,
        string Email,
        string FirstName,
        string LastName,
        bool Enabled,
        bool EmailVerified,
        KeycloakCredentialRepresentation[] Credentials,
        string[] RequiredActions);

    private sealed record KeycloakCredentialRepresentation(
        string Type,
        string Value,
        bool Temporary);

    private sealed record RoleRepresentation(
        string? Id,
        string Name,
        string? Description,
        bool Composite,
        bool ClientRole,
        string? ContainerId);
}
