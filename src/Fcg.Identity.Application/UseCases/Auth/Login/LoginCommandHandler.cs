using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Application.Audit;
using Fcg.Identity.Domain.DonorProfiles;
using Fcg.Identity.Domain.ManagerProfiles;
using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.Application.UseCases.Auth.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IDonorProfileRepository _donorProfileRepository;
    private readonly IManagerProfileRepository _managerProfileRepository;

    public LoginCommandHandler(
        IIdentityProvider identityProvider,
        IMessagePublisher messagePublisher,
        IDonorProfileRepository donorProfileRepository,
        IManagerProfileRepository managerProfileRepository)
    {
        _identityProvider = identityProvider;
        _messagePublisher = messagePublisher;
        _donorProfileRepository = donorProfileRepository;
        _managerProfileRepository = managerProfileRepository;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var loginResult = await _identityProvider.LoginAsync(new LoginIdentityUserRequest(command.Email, command.Password), cancellationToken);

        if (loginResult.IsFailure)
        {
            AddAuditLog(AuditActions.LoginFailed, command.Email);
            return loginResult.Error;
        }

        await AddAuditLogAsync(AuditActions.LoginSucceeded, command.Email, loginResult.Value, cancellationToken);

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

    private async Task AddAuditLogAsync(
        string action,
        string email,
        LoginIdentityUserResponse token,
        CancellationToken cancellationToken)
    {
        var actor = await AuditActorResolver.ResolveAsync(
            token,
            _donorProfileRepository,
            _managerProfileRepository,
            cancellationToken);

        _messagePublisher.PublishAuditLogFireAndForget(
            AuditLogRequestedEvent.Create(
                action,
                "Authentication",
                actor.ActorId,
                actor.ActorType,
                metadata: new Dictionary<string, object?> { ["email"] = email }));
    }
}
