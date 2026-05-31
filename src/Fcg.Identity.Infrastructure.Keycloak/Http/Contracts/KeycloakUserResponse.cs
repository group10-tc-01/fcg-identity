using System.Text.Json.Serialization;

namespace Fcg.Identity.Infrastructure.Keycloak.Http.Contracts;

public sealed record KeycloakUserResponse([property: JsonPropertyName("id")] string Id);
