namespace Fcg.Identity.WebApi.Controllers.v1;

public sealed record RegisterDonorRequest(
    string FullName,
    string Email,
    string Cpf,
    string Password);
