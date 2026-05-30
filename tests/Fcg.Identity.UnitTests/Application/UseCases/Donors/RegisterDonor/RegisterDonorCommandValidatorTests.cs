using Fcg.Identity.Application.UseCases.Donors.RegisterDonor;
using Fcg.Identity.CommomTestsUtilities.Builders.Donors;
using FluentAssertions;

namespace Fcg.Identity.UnitTests.Application.UseCases.Donors.RegisterDonor;

public sealed class RegisterDonorCommandValidatorTests
{
    [Fact]
    public void Given_Validate_Called_When_CommandIsValid_Then_ShouldBeValid()
    {
        // Arrange
        var validator = new RegisterDonorCommandValidator();
        var command = new RegisterDonorCommandBuilder().Build();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Given_Validate_Called_When_FullNameIsEmpty_Then_ShouldReturnValidationFailure()
    {
        // Arrange
        var validator = new RegisterDonorCommandValidator();
        var command = new RegisterDonorCommandBuilder()
            .WithFullName(string.Empty)
            .Build();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Full name is required.");
    }

    [Fact]
    public void Given_Validate_Called_When_EmailIsInvalid_Then_ShouldReturnValidationFailure()
    {
        // Arrange
        var validator = new RegisterDonorCommandValidator();
        var command = new RegisterDonorCommandBuilder()
            .WithEmail("invalid-email")
            .Build();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Email is invalid.");
    }

    [Fact]
    public void Given_Validate_Called_When_CpfIsInvalid_Then_ShouldReturnValidationFailure()
    {
        // Arrange
        var validator = new RegisterDonorCommandValidator();
        var command = new RegisterDonorCommandBuilder()
            .WithCpf("11111111111")
            .Build();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "CPF is invalid.");
    }

    [Fact]
    public void Given_Validate_Called_When_PasswordIsTooShort_Then_ShouldReturnValidationFailure()
    {
        // Arrange
        var validator = new RegisterDonorCommandValidator();
        var command = new RegisterDonorCommandBuilder()
            .WithPassword("123")
            .Build();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == "Password must have at least 8 characters.");
    }
}
