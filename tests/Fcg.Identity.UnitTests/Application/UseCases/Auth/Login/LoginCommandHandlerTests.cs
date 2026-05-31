using Fcg.Identity.Application.Abstractions.Identity;
using Fcg.Identity.Application.UseCases.Auth.Login;
using Fcg.Identity.CommomTestsUtilities.TestDoubles;
using Fcg.Identity.Domain.Shared.Results;
using FluentAssertions;

namespace Fcg.Identity.UnitTests.Application.UseCases.Auth.Login;

public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task Given_Handle_Called_When_CredentialsAreValid_Then_ShouldReturnTokenResponse()
    {
        // Arrange
        var identityProvider = new FakeIdentityProvider();
        identityProvider.ConfigureLoginResult(new LoginIdentityUserResponse("access-token", "refresh-token", 300, "Bearer"));
        var auditLogRepository = new InMemoryAuditLogRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new LoginCommandHandler(identityProvider, auditLogRepository, unitOfWork);
        var command = new LoginCommand("doador@email.com", "Password123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("access-token");
        result.Value.RefreshToken.Should().Be("refresh-token");
        result.Value.ExpiresIn.Should().Be(300);
        result.Value.TokenType.Should().Be("Bearer");
        identityProvider.LoginCalls.Should().Be(1);
        identityProvider.LastLoginRequest.Should().BeEquivalentTo(new LoginIdentityUserRequest(command.Email, command.Password));
        auditLogRepository.AuditLogs.Should().ContainSingle(auditLog => auditLog.Action == "LoginSucceeded");
        unitOfWork.SaveChangesCalls.Should().Be(1);
    }

    [Fact]
    public async Task Given_Handle_Called_When_IdentityProviderFails_Then_ShouldReturnFailure()
    {
        // Arrange
        var identityProvider = new FakeIdentityProvider();
        identityProvider.ConfigureLoginResult(Error.Unauthorized("IdentityProvider.InvalidCredentials", "Invalid email or password."));
        var auditLogRepository = new InMemoryAuditLogRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new LoginCommandHandler(identityProvider, auditLogRepository, unitOfWork);
        var command = new LoginCommand("doador@email.com", "wrong-password");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("IdentityProvider.InvalidCredentials");
        identityProvider.LoginCalls.Should().Be(1);
        auditLogRepository.AuditLogs.Should().ContainSingle(auditLog => auditLog.Action == "LoginFailed");
        unitOfWork.SaveChangesCalls.Should().Be(1);
    }
}
