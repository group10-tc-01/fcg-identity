using Fcs.Identity.Application.Abstractions.Messaging;

namespace Fcs.Identity.Application.UseCases.Donors.RegisterDonor;

public sealed record RegisterDonorCommand(
    string FullName,
    string Email,
    string Cpf,
    string Password) : ICommand<RegisterDonorResponse>;
