namespace Fcs.Identity.Application.Abstractions.Identity;

public sealed record CreateDonorIdentityUserRequest(
    string FullName,
    string Email,
    string Password);
