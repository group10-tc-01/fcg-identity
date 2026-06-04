using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.Application.Abstractions.Identity;

public interface IIdentityProvider
{
    Task<Result<CreateDonorIdentityUserResponse>> CreateDonorAsync(
        CreateDonorIdentityUserRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<EnsureManagerIdentityUserResponse>> EnsureManagerAsync(
        EnsureManagerIdentityUserRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<LoginIdentityUserResponse>> LoginAsync(
        LoginIdentityUserRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<LoginIdentityUserResponse>> RefreshTokenAsync(
        RefreshTokenIdentityUserRequest request,
        CancellationToken cancellationToken = default);
}
