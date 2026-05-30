using Fcg.Identity.Domain.Results;

namespace Fcg.Identity.Domain.DonorProfiles;

public sealed class DonorProfile
{
    private DonorProfile()
    {
    }

    private DonorProfile(
        Guid id,
        string keycloakUserId,
        string fullName,
        string email,
        string cpf,
        DateTime createdAt)
    {
        Id = id;
        KeycloakUserId = keycloakUserId;
        FullName = fullName;
        Email = email;
        Cpf = cpf;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string KeycloakUserId { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static Result<DonorProfile> Create(string keycloakUserId, string fullName, string email, string cpf)
    {
        var normalizedKeycloakUserId = keycloakUserId?.Trim() ?? string.Empty;
        var normalizedFullName = fullName?.Trim() ?? string.Empty;
        var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
        var normalizedCpf = cpf?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedKeycloakUserId))
        {
            return Error.Validation("DonorProfile.KeycloakUserIdRequired", "Keycloak user id is required.");
        }

        if (string.IsNullOrWhiteSpace(normalizedFullName))
        {
            return Error.Validation("DonorProfile.FullNameRequired", "Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return Error.Validation("DonorProfile.EmailRequired", "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(normalizedCpf))
        {
            return Error.Validation("DonorProfile.CpfRequired", "CPF is required.");
        }

        return new DonorProfile(
            Guid.NewGuid(),
            normalizedKeycloakUserId,
            normalizedFullName,
            normalizedEmail,
            normalizedCpf,
            DateTime.UtcNow);
    }

    public void Update(string fullName, string email)
    {
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        UpdatedAt = DateTime.UtcNow;
    }
}
