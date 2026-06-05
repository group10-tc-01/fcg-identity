namespace Fcs.Identity.Application.Abstractions.Identity;

public sealed record EnsureManagerIdentityUserRequest(
    string FullName,
    string Email,
    string Password);
