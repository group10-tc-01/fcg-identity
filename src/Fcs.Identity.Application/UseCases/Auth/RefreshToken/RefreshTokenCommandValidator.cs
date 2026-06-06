using Fcs.Identity.Resources.Messages;
using FluentValidation;

namespace Fcs.Identity.Application.UseCases.Auth.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(command => command.RefreshToken)
            .NotEmpty()
            .WithMessage(IdentityMessages.RefreshTokenRequired);
    }
}
