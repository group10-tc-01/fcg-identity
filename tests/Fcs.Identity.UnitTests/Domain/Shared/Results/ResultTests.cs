using Fcs.Identity.Domain.Shared.Results;
using FluentAssertions;

namespace Fcs.Identity.UnitTests.Domain.Shared.Results;

public sealed class ResultTests
{
    [Fact]
    public void Given_Success_Called_When_ValueIsProvided_Then_ShouldReturnSuccessResult()
    {
        // Arrange
        const string value = "success";

        // Act
        var result = Result<string>.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void Given_Failure_Called_When_ErrorIsProvided_Then_ShouldReturnFailureResult()
    {
        // Arrange
        var error = Error.Validation("Validation.Failed", "Validation failed.");

        // Act
        var result = Result<string>.Failure(error);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Given_Value_Accessed_When_ResultIsFailure_Then_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var result = Result<string>.Failure(Error.Failure("Error", "Error message."));

        // Act
        var act = () => result.Value;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot access Value of a failed Result.");
    }

    [Fact]
    public void Given_Error_Accessed_When_ResultIsSuccess_Then_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var result = Result<string>.Success("success");

        // Act
        var act = () => result.Error;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot access Error of a successful Result.");
    }

    [Fact]
    public void Given_Match_Called_When_ResultIsSuccess_Then_ShouldRunSuccessDelegate()
    {
        // Arrange
        var result = Result<string>.Success("success");

        // Act
        var output = result.Match(value => value.ToUpperInvariant(), error => error.Code);

        // Assert
        output.Should().Be("SUCCESS");
    }

    [Fact]
    public void Given_Match_Called_When_ResultIsFailure_Then_ShouldRunFailureDelegate()
    {
        // Arrange
        var result = Result<string>.Failure(Error.Failure("Error", "Error message."));

        // Act
        var output = result.Match(value => value, error => error.Code);

        // Assert
        output.Should().Be("Error");
    }
}
