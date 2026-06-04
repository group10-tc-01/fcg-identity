using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.CommomTestsUtilities.TestDoubles;

public sealed class FakeIdentityProvider : IIdentityProvider
{
    private Result<CreateDonorIdentityUserResponse> _createDonorResult =
        new CreateDonorIdentityUserResponse(Guid.NewGuid().ToString());

    private Result<EnsureManagerIdentityUserResponse> _ensureManagerResult =
        new EnsureManagerIdentityUserResponse("manager-keycloak-user-id");

    private Result<LoginIdentityUserResponse> _loginResult =
        new LoginIdentityUserResponse("access-token", "refresh-token", 300, "Bearer");

    private Result<LoginIdentityUserResponse> _refreshTokenResult =
        new LoginIdentityUserResponse("new-access-token", "new-refresh-token", 300, "Bearer");

    public int CreateDonorCalls { get; private set; }
    public CreateDonorIdentityUserRequest? LastCreateDonorRequest { get; private set; }

    public int EnsureManagerCalls { get; private set; }
    public EnsureManagerIdentityUserRequest? LastEnsureManagerRequest { get; private set; }

    public int LoginCalls { get; private set; }
    public LoginIdentityUserRequest? LastLoginRequest { get; private set; }

    public int RefreshTokenCalls { get; private set; }
    public RefreshTokenIdentityUserRequest? LastRefreshTokenRequest { get; private set; }

    public void Reset()
    {
        _createDonorResult = new CreateDonorIdentityUserResponse(Guid.NewGuid().ToString());
        _ensureManagerResult = new EnsureManagerIdentityUserResponse("manager-keycloak-user-id");
        _loginResult = new LoginIdentityUserResponse("access-token", "refresh-token", 300, "Bearer");
        _refreshTokenResult = new LoginIdentityUserResponse("new-access-token", "new-refresh-token", 300, "Bearer");
        CreateDonorCalls = 0;
        LastCreateDonorRequest = null;
        EnsureManagerCalls = 0;
        LastEnsureManagerRequest = null;
        LoginCalls = 0;
        LastLoginRequest = null;
        RefreshTokenCalls = 0;
        LastRefreshTokenRequest = null;
    }

    public void ConfigureCreateDonorResult(Result<CreateDonorIdentityUserResponse> result)
    {
        _createDonorResult = result;
    }

    public void ConfigureEnsureManagerResult(Result<EnsureManagerIdentityUserResponse> result)
    {
        _ensureManagerResult = result;
    }

    public void ConfigureLoginResult(Result<LoginIdentityUserResponse> result)
    {
        _loginResult = result;
    }

    public void ConfigureRefreshTokenResult(Result<LoginIdentityUserResponse> result)
    {
        _refreshTokenResult = result;
    }

    public Task<Result<CreateDonorIdentityUserResponse>> CreateDonorAsync(
        CreateDonorIdentityUserRequest request,
        CancellationToken cancellationToken = default)
    {
        CreateDonorCalls++;
        LastCreateDonorRequest = request;

        return Task.FromResult(_createDonorResult);
    }

    public Task<Result<EnsureManagerIdentityUserResponse>> EnsureManagerAsync(
        EnsureManagerIdentityUserRequest request,
        CancellationToken cancellationToken = default)
    {
        EnsureManagerCalls++;
        LastEnsureManagerRequest = request;

        return Task.FromResult(_ensureManagerResult);
    }

    public Task<Result<LoginIdentityUserResponse>> LoginAsync(
        LoginIdentityUserRequest request,
        CancellationToken cancellationToken = default)
    {
        LoginCalls++;
        LastLoginRequest = request;

        return Task.FromResult(_loginResult);
    }

    public Task<Result<LoginIdentityUserResponse>> RefreshTokenAsync(
        RefreshTokenIdentityUserRequest request,
        CancellationToken cancellationToken = default)
    {
        RefreshTokenCalls++;
        LastRefreshTokenRequest = request;

        return Task.FromResult(_refreshTokenResult);
    }
}
