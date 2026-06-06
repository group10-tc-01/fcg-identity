using System.Net.Mail;
using Fcs.Identity.Domain.Shared.Results;
using Fcs.Identity.Resources.Messages;

namespace Fcs.Identity.Domain.Shared.ValueObjects;

public readonly record struct Email
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Email> Create(string? email)
    {
        var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return Error.Validation(IdentityErrorCodes.EmailRequired, IdentityMessages.EmailRequired);
        }

        if (normalizedEmail.Length > 320 || !IsValidEmail(normalizedEmail))
        {
            return Error.Validation(IdentityErrorCodes.EmailInvalid, IdentityMessages.EmailInvalid);
        }

        return new Email(normalizedEmail);
    }

    public override string ToString() => Value;

    private static bool IsValidEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
            return mailAddress.Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
