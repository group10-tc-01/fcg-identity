namespace Fcg.Identity.Application.UseCases.Donors.RegisterDonor;

public sealed record RegisterDonorResponse(
    Guid Id,
    string FullName,
    string Email,
    string Cpf);
