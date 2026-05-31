using Fcg.Identity.CommomTestsUtilities.Fakers.Shared;
using Fcg.Identity.Domain.ManagerProfiles;
using FluentAssertions;

namespace Fcg.Identity.UnitTests.Domain.ManagerProfiles;

public sealed class ManagerProfileTests
{
    [Fact]
    public void Given_Create_Called_When_ProfileDataIsValid_Then_ShouldCreateManagerProfile()
    {
        // Arrange
        var keycloakUserId = Guid.NewGuid().ToString();
        var fullName = "Gestor ONG";
        var email = EmailFaker.Generate();

        // Act
        var result = ManagerProfile.Create(keycloakUserId, fullName, email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.KeycloakUserId.Should().Be(keycloakUserId);
        result.Value.FullName.Should().Be(fullName);
        result.Value.Email.Value.Should().Be(email);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Given_Create_Called_When_KeycloakUserIdIsEmpty_Then_ShouldReturnFailure()
    {
        // Arrange
        var keycloakUserId = string.Empty;

        // Act
        var result = ManagerProfile.Create(keycloakUserId, "Gestor ONG", EmailFaker.Generate());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ManagerProfile.KeycloakUserIdRequired");
    }

    [Fact]
    public void Given_Update_Called_When_ProfileDataIsValid_Then_ShouldUpdateManagerProfile()
    {
        // Arrange
        var managerProfile = ManagerProfile.Create(Guid.NewGuid().ToString(), "Gestor ONG", EmailFaker.Generate()).Value;
        var newEmail = EmailFaker.Generate();

        // Act
        var result = managerProfile.Update("Gestor Atualizado", newEmail);

        // Assert
        result.IsSuccess.Should().BeTrue();
        managerProfile.FullName.Should().Be("Gestor Atualizado");
        managerProfile.Email.Value.Should().Be(newEmail);
        managerProfile.UpdatedAt.Should().NotBeNull();
    }
}
