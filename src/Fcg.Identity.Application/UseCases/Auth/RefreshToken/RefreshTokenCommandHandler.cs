using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Application.Audit;
using Fcg.Identity.Application.UseCases.Auth.Login;
using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.Application.UseCases.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IMessagePublisher _messagePublisher;

    public RefreshTokenCommandHandler(
        IIdentityProvider identityProvider,
        IMessagePublisher messagePublisher)
    {
        _identityProvider = identityProvider;
        _messagePublisher = messagePublisher;
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

        _messagePublisher.PublishAuditLogFireAndForget(
            AuditLogRequestedEvent.Create(AuditActions.TokenRefreshed, "Authentication", actorType: "Public"));

        return new LoginResponse(
            refreshResult.Value.AccessToken,
            refreshResult.Value.RefreshToken,
            refreshResult.Value.ExpiresIn,
            refreshResult.Value.TokenType);
    }
}
