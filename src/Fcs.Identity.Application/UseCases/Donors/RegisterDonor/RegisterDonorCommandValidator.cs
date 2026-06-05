using Fcs.Identity.Domain.Shared.ValueObjects;
using FluentValidation;

namespace Fcs.Identity.Application.UseCases.Donors.RegisterDonor;

public sealed class RegisterDonorCommandValidator : AbstractValidator<RegisterDonorCommand>
{
    public RegisterDonorCommandValidator()
    {
        RuleFor(command => command.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.");

        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Must(email => Email.Create(email).IsSuccess)
            .WithMessage("Email is invalid.");

        RuleFor(command => command.Cpf)
            .NotEmpty()
            .WithMessage("CPF is required.")
            .Must(cpf => Cpf.Create(cpf).IsSuccess)
            .WithMessage("CPF is invalid.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must have at least 8 characters.");
    }
}
