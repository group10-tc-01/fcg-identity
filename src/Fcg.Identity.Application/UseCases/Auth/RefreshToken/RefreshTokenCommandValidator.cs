using FluentValidation;

namespace Fcg.Identity.Application.UseCases.Auth.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(command => command.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}
