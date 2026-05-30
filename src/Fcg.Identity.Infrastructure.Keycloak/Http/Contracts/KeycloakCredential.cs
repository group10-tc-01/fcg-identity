using System.Text.Json.Serialization;

namespace Fcg.Identity.Infrastructure.Keycloak.Http.Contracts;

public sealed record KeycloakCredential(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("value")] string Value,
    [property: JsonPropertyName("temporary")] bool Temporary);
