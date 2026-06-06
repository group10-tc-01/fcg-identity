using Fcs.Identity.Domain.Shared.ValueObjects;
using Fcs.Identity.Resources.Messages;
using FluentValidation;

namespace Fcs.Identity.Application.UseCases.Donors.RegisterDonor;

public sealed class RegisterDonorCommandValidator : AbstractValidator<RegisterDonorCommand>
{
    public RegisterDonorCommandValidator()
    {
        RuleFor(command => command.FullName)
            .NotEmpty()
            .WithMessage(IdentityMessages.FullNameRequired);

        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage(IdentityMessages.EmailRequired)
            .Must(email => Email.Create(email).IsSuccess)
            .WithMessage(IdentityMessages.EmailInvalid);

        RuleFor(command => command.Cpf)
            .NotEmpty()
            .WithMessage(IdentityMessages.CpfRequired)
            .Must(cpf => Cpf.Create(cpf).IsSuccess)
            .WithMessage(IdentityMessages.CpfInvalid);

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage(IdentityMessages.PasswordRequired)
            .MinimumLength(8)
            .WithMessage(IdentityMessages.PasswordMinimumLength);
    }
}
