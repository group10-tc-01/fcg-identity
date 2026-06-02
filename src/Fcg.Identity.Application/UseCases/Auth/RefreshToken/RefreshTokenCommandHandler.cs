using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Application.Audit;
using Fcg.Identity.Application.UseCases.Auth.Login;
using Fcg.Identity.Domain.DonorProfiles;
using Fcg.Identity.Domain.ManagerProfiles;
using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.Application.UseCases.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IDonorProfileRepository _donorProfileRepository;
    private readonly IManagerProfileRepository _managerProfileRepository;

    public RefreshTokenCommandHandler(
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

    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshResult = await _identityProvider.RefreshTokenAsync(
            new RefreshTokenIdentityUserRequest(command.RefreshToken),
            cancellationToken);

        if (refreshResult.IsFailure)
        {
            return refreshResult.Error;
        }

        var actor = await AuditActorResolver.ResolveAsync(
            refreshResult.Value,
            _donorProfileRepository,
            _managerProfileRepository,
            cancellationToken);

        _messagePublisher.PublishAuditLogFireAndForget(
            AuditLogRequestedEvent.Create(
                AuditActions.TokenRefreshed,
                "Authentication",
                actor.ActorId,
                actor.ActorType));

        return new LoginResponse(
            refreshResult.Value.AccessToken,
            refreshResult.Value.RefreshToken,
            refreshResult.Value.ExpiresIn,
            refreshResult.Value.TokenType);
    }
}
