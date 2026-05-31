using Fcg.Identity.Domain.Shared.ValueObjects;
using FluentValidation;

namespace Fcg.Identity.Application.UseCases.Auth.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Must(email => Email.Create(email).IsSuccess)
            .WithMessage("Email is invalid.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}
