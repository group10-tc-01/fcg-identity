using Fcs.Identity.Domain.Shared.ValueObjects;
using Fcs.Identity.Resources.Messages;
using FluentValidation;

namespace Fcs.Identity.Application.UseCases.Auth.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage(IdentityMessages.EmailRequired)
            .Must(email => Email.Create(email).IsSuccess)
            .WithMessage(IdentityMessages.EmailInvalid);

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage(IdentityMessages.PasswordRequired);
    }
}
