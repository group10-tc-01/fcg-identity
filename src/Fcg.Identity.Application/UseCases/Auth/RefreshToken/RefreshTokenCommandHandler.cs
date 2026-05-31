using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Application.UseCases.Auth.Login;
using Fcg.Identity.Domain.Abstractions;
using Fcg.Identity.Domain.AuditLogs;
using Fcg.Identity.Domain.Shared.Results;
using Microsoft.Extensions.Logging;

namespace Fcg.Identity.Application.UseCases.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IIdentityProvider identityProvider,
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _identityProvider = identityProvider;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refresh token flow started");
        _logger.LogInformation("Refreshing token with identity provider");

        var refreshResult = await _identityProvider.RefreshTokenAsync(
            new RefreshTokenIdentityUserRequest(command.RefreshToken),
            cancellationToken);

        if (refreshResult.IsFailure)
        {
            _logger.LogWarning(
                "Refresh token flow failed. ErrorCode: {ErrorCode}",
                refreshResult.Error.Code);

            return refreshResult.Error;
        }

        _logger.LogInformation("Refresh token flow succeeded. Persisting audit log");
        await _auditLogRepository.AddAsync(
            AuditLog.Create(AuditActions.TokenRefreshed, "Authentication", actorType: "Public").Value,
            cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Refresh token flow completed. TokenType: {TokenType}. ExpiresIn: {ExpiresIn}",
            refreshResult.Value.TokenType,
            refreshResult.Value.ExpiresIn);

        return new LoginResponse(
            refreshResult.Value.AccessToken,
            refreshResult.Value.RefreshToken,
            refreshResult.Value.ExpiresIn,
            refreshResult.Value.TokenType);
    }
}
