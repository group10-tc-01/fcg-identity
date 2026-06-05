using System.Text.Json.Serialization;

namespace Fcs.Identity.Infrastructure.Keycloak.Http.Contracts;

public sealed record KeycloakUserResponse([property: JsonPropertyName("id")] string Id);
