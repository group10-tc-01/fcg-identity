using Fcg.Identity.Application.UseCases.Auth.Login;
using FluentAssertions;

namespace Fcg.Identity.UnitTests.Application.UseCases.Auth.Login;

public sealed class LoginCommandValidatorTests
{
    [Fact]
    public void Given_Validate_Called_When_CommandIsValid_Then_ShouldBeValid()
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("doador@email.com", "Password123!");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Given_Validate_Called_When_EmailIsEmpty_Then_ShouldReturnValidationFailure()
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var command = new LoginCommand(string.Empty, "Password123!");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Email is required.");
    }

    [Fact]
    public void Given_Validate_Called_When_EmailIsInvalid_Then_ShouldReturnValidationFailure()
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("invalid-email", "Password123!");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Email is invalid.");
    }

    [Fact]
    public void Given_Validate_Called_When_PasswordIsEmpty_Then_ShouldReturnValidationFailure()
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("doador@email.com", string.Empty);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Password is required.");
    }
}
