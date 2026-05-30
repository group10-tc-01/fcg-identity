using System.Text.Json.Serialization;

namespace Fcg.Identity.Infrastructure.Keycloak.Http.Contracts;

public sealed record KeycloakRoleResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name);
