using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Domain.Abstractions;
using Fcg.Identity.Domain.AuditLogs;
using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.Application.UseCases.Auth.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IIdentityProvider identityProvider,
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork)
    {
        _identityProvider = identityProvider;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var loginResult = await _identityProvider.LoginAsync(new LoginIdentityUserRequest(command.Email, command.Password), cancellationToken);

        if (loginResult.IsFailure)
        {
            await AddAuditLogAsync(AuditActions.LoginFailed, command.Email, cancellationToken);
            return loginResult.Error;
        }

        await AddAuditLogAsync(AuditActions.LoginSucceeded, command.Email, cancellationToken);

        return new LoginResponse(
            loginResult.Value.AccessToken,
            loginResult.Value.RefreshToken,
            loginResult.Value.ExpiresIn,
            loginResult.Value.TokenType);
    }

    private async Task AddAuditLogAsync(string action, string email, CancellationToken cancellationToken)
    {
        await _auditLogRepository.AddAsync(
            AuditLog.Create(
                action,
                "Authentication",
                actorType: "Public",
                metadataJson: $$"""{"email":"{{email}}"}""").Value,
            cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
