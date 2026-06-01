using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Application.Audit;
using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.Application.UseCases.Auth.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IMessagePublisher _messagePublisher;

    public LoginCommandHandler(
        IIdentityProvider identityProvider,
        IMessagePublisher messagePublisher)
    {
        _identityProvider = identityProvider;
        _messagePublisher = messagePublisher;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var loginResult = await _identityProvider.LoginAsync(new LoginIdentityUserRequest(command.Email, command.Password), cancellationToken);

        if (loginResult.IsFailure)
        {
            AddAuditLog(AuditActions.LoginFailed, command.Email);
            return loginResult.Error;
        }

        AddAuditLog(AuditActions.LoginSucceeded, command.Email);

        return new LoginResponse(
            loginResult.Value.AccessToken,
            loginResult.Value.RefreshToken,
            loginResult.Value.ExpiresIn,
            loginResult.Value.TokenType);
    }

    private void AddAuditLog(string action, string email)
    {
        _messagePublisher.PublishAuditLogFireAndForget(
            AuditLogRequestedEvent.Create(
                action,
                "Authentication",
                actorType: "Public",
                metadata: new Dictionary<string, object?> { ["email"] = email }));
    }
}
