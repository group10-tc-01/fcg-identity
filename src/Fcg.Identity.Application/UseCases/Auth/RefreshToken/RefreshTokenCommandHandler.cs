using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Application.UseCases.Auth.Login;
using Fcg.Identity.Domain.Abstractions;
using Fcg.Identity.Domain.AuditLogs;
using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.Application.UseCases.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IIdentityProvider identityProvider,
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork)
    {
        _identityProvider = identityProvider;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
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

        await _auditLogRepository.AddAsync(
            AuditLog.Create(AuditActions.TokenRefreshed, "Authentication", actorType: "Public").Value,
            cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            refreshResult.Value.AccessToken,
            refreshResult.Value.RefreshToken,
            refreshResult.Value.ExpiresIn,
            refreshResult.Value.TokenType);
    }
}
