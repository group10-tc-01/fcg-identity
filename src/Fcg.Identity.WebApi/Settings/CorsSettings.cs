using System.ComponentModel.DataAnnotations;

namespace Fcg.Identity.WebApi.Settings;

public sealed class CorsSettings
{
    public const string SectionName = "Cors";

    [Required]
    public string[] AllowedOrigins { get; init; } = [];
}
