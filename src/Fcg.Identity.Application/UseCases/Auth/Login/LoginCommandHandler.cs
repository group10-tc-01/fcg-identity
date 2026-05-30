using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.Application.UseCases.Auth.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityProvider _identityProvider;

    public LoginCommandHandler(IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var loginResult = await _identityProvider.LoginAsync(new LoginIdentityUserRequest(command.Email, command.Password), cancellationToken);

        if (loginResult.IsFailure)
        {
            return loginResult.Error;
        }

        return new LoginResponse(
            loginResult.Value.AccessToken,
            loginResult.Value.RefreshToken,
            loginResult.Value.ExpiresIn,
            loginResult.Value.TokenType);
    }
}
