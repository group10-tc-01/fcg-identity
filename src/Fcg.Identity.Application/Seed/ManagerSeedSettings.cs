using System.ComponentModel.DataAnnotations;

namespace Fcg.Identity.Application.Seed;

public sealed class ManagerSeedSettings
{
    public const string SectionName = "ManagerSeed";

    public bool Enabled { get; set; }

    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
