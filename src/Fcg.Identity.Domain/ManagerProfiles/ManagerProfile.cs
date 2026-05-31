using Fcg.Identity.Domain.Abstractions;
using Fcg.Identity.Domain.Shared.Results;
using Fcg.Identity.Domain.Shared.ValueObjects;

namespace Fcg.Identity.Domain.ManagerProfiles;

public sealed class ManagerProfile : BaseEntity
{
    private ManagerProfile()
    {
    }

    private ManagerProfile(Guid id, string keycloakUserId, string fullName, Email email) : base(id)
    {
        KeycloakUserId = keycloakUserId;
        FullName = fullName;
        Email = email;
    }

    public string KeycloakUserId { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public Email Email { get; private set; }

    public static Result<ManagerProfile> Create(string keycloakUserId, string fullName, string email)
    {
        var normalizedKeycloakUserId = keycloakUserId?.Trim() ?? string.Empty;
        var normalizedFullName = fullName?.Trim() ?? string.Empty;
        var normalizedEmail = email?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedKeycloakUserId))
        {
            return Error.Validation("ManagerProfile.KeycloakUserIdRequired", "Keycloak user id is required.");
        }

        if (string.IsNullOrWhiteSpace(normalizedFullName))
        {
            return Error.Validation("ManagerProfile.FullNameRequired", "Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return Error.Validation("ManagerProfile.EmailRequired", "Email is required.");
        }

        var emailResult = Email.Create(normalizedEmail);
        if (emailResult.IsFailure)
        {
            return emailResult.Error;
        }

        return new ManagerProfile(
            Guid.NewGuid(),
            normalizedKeycloakUserId,
            normalizedFullName,
            emailResult.Value);
    }

    public Result<ManagerProfile> Update(string fullName, string email)
    {
        var normalizedFullName = fullName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedFullName))
        {
            return Error.Validation("ManagerProfile.FullNameRequired", "Full name is required.");
        }

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
        {
            return emailResult.Error;
        }

        FullName = normalizedFullName;
        Email = emailResult.Value;
        UpdatedAt = DateTime.UtcNow;

        return this;
    }
}
