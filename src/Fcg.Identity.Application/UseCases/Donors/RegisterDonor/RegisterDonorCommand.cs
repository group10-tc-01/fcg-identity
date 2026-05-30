using Fcg.Identity.Application.Abstractions.Messaging;

namespace Fcg.Identity.Application.UseCases.Donors.RegisterDonor;

public sealed record RegisterDonorCommand(
    string FullName,
    string Email,
    string Cpf,
    string Password) : ICommand<RegisterDonorResponse>;
