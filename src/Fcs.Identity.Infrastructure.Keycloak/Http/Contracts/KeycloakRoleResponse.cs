using System.Text.Json.Serialization;

namespace Fcs.Identity.Infrastructure.Keycloak.Http.Contracts;

public sealed record KeycloakRoleResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name);
