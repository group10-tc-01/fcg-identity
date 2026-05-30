using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.CommomTestsUtilities.TestDoubles;

public sealed class FakeIdentityProvider : IIdentityProvider
{
    private Result<CreateDonorIdentityUserResponse> _createDonorResult =
        new CreateDonorIdentityUserResponse(Guid.NewGuid().ToString());

    public int CreateDonorCalls { get; private set; }
    public CreateDonorIdentityUserRequest? LastCreateDonorRequest { get; private set; }

    public void ConfigureCreateDonorResult(Result<CreateDonorIdentityUserResponse> result)
    {
        _createDonorResult = result;
    }

    public Task<Result<CreateDonorIdentityUserResponse>> CreateDonorAsync(
        CreateDonorIdentityUserRequest request,
        CancellationToken cancellationToken = default)
    {
        CreateDonorCalls++;
        LastCreateDonorRequest = request;

        return Task.FromResult(_createDonorResult);
    }
}
