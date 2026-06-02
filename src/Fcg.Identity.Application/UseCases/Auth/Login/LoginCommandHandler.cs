using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.Abstractions.Messaging;
using Fcg.Identity.Domain.Abstractions;
using Fcg.Identity.Domain.AuditLogs;
using Fcg.Identity.Domain.Shared.Results;
using Microsoft.Extensions.Logging;

namespace Fcg.Identity.Application.UseCases.Auth.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IIdentityProvider identityProvider,
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork,
        ILogger<LoginCommandHandler> logger)
    {
        _identityProvider = identityProvider;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login flow started. Email: {Email}", command.Email);
        _logger.LogInformation("Authenticating user with identity provider. Email: {Email}", command.Email);

        var loginResult = await _identityProvider.LoginAsync(new LoginIdentityUserRequest(command.Email, command.Password), cancellationToken);

        if (loginResult.IsFailure)
        {
            _logger.LogWarning(
                "Login flow failed for email {Email}. ErrorCode: {ErrorCode}",
                command.Email,
                loginResult.Error.Code);

            await AddAuditLogAsync(AuditActions.LoginFailed, command.Email, cancellationToken);
            return loginResult.Error;
        }

        _logger.LogInformation("Login flow succeeded for email {Email}. Persisting audit log", command.Email);
        await AddAuditLogAsync(AuditActions.LoginSucceeded, command.Email, cancellationToken);

        _logger.LogInformation(
            "Login flow completed for email {Email}. TokenType: {TokenType}. ExpiresIn: {ExpiresIn}",
            command.Email,
            loginResult.Value.TokenType,
            loginResult.Value.ExpiresIn);

        return new LoginResponse(
            loginResult.Value.AccessToken,
            loginResult.Value.RefreshToken,
            loginResult.Value.ExpiresIn,
            loginResult.Value.TokenType);
    }

    private async Task AddAuditLogAsync(string action, string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Writing login audit log. Action: {AuditAction}. Email: {Email}", action, email);

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
