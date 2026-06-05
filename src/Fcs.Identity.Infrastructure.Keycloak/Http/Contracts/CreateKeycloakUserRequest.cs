using System.Text.Json.Serialization;

namespace Fcs.Identity.Infrastructure.Keycloak.Http.Contracts;

public sealed record CreateKeycloakUserRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("firstName")] string FirstName,
    [property: JsonPropertyName("lastName")] string LastName,
    [property: JsonPropertyName("enabled")] bool Enabled,
    [property: JsonPropertyName("emailVerified")] bool EmailVerified,
    [property: JsonPropertyName("credentials")] IReadOnlyCollection<KeycloakCredential> Credentials,
    [property: JsonPropertyName("requiredActions")] IReadOnlyCollection<string> RequiredActions);
