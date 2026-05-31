using Fcg.Identity.Application.UseCases.Auth.RefreshToken;
using FluentAssertions;

namespace Fcg.Identity.UnitTests.Application.UseCases.Auth.RefreshToken;

public sealed class RefreshTokenCommandValidatorTests
{
    [Fact]
    public void Given_Validate_Called_When_RefreshTokenIsProvided_Then_ShouldBeValid()
    {
        // Arrange
        var validator = new RefreshTokenCommandValidator();
        var command = new RefreshTokenCommand("refresh-token");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Given_Validate_Called_When_RefreshTokenIsEmpty_Then_ShouldBeInvalid()
    {
        // Arrange
        var validator = new RefreshTokenCommandValidator();
        var command = new RefreshTokenCommand(string.Empty);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.ErrorMessage == "Refresh token is required.");
    }
}
