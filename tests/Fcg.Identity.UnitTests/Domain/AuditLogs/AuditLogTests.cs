using Fcg.Identity.Domain.AuditLogs;
using FluentAssertions;

namespace Fcg.Identity.UnitTests.Domain.AuditLogs;

public sealed class AuditLogTests
{
    [Fact]
    public void Given_Create_Called_When_AuditDataIsValid_Then_ShouldCreateAuditLog()
    {
        // Arrange
        var actorId = Guid.NewGuid();

        // Act
        var result = AuditLog.Create(
            AuditActions.DonorRegistered,
            "DonorProfile",
            actorId,
            "Doador",
            entityId: actorId.ToString());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.Action.Should().Be(AuditActions.DonorRegistered);
        result.Value.EntityName.Should().Be("DonorProfile");
        result.Value.ActorId.Should().Be(actorId);
        result.Value.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
    }

    [Fact]
    public void Given_Create_Called_When_ActionIsEmpty_Then_ShouldReturnFailure()
    {
        // Arrange

        // Act
        var result = AuditLog.Create(string.Empty, "DonorProfile");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AuditLog.ActionRequired");
    }

    [Fact]
    public void Given_Create_Called_When_EntityNameIsEmpty_Then_ShouldReturnFailure()
    {
        // Arrange

        // Act
        var result = AuditLog.Create(AuditActions.DonorRegistered, string.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("AuditLog.EntityNameRequired");
    }
}
