using Fcg.Identity.Domain.Shared.Results;
using FluentAssertions;

namespace Fcg.Identity.UnitTests.Domain.Shared.Results;

public sealed class ErrorTests
{
    [Theory]
    [InlineData(nameof(Error.Failure), ErrorType.Failure)]
    [InlineData(nameof(Error.Validation), ErrorType.Validation)]
    [InlineData(nameof(Error.NotFound), ErrorType.NotFound)]
    [InlineData(nameof(Error.Conflict), ErrorType.Conflict)]
    [InlineData(nameof(Error.Unauthorized), ErrorType.Unauthorized)]
    public void Given_ErrorFactory_Called_When_CodeAndMessageAreProvided_Then_ShouldReturnExpectedErrorType(
        string factoryName,
        ErrorType expectedType)
    {
        // Arrange
        const string code = "Error.Code";
        const string message = "Error message.";

        // Act
        var error = CreateError(factoryName, code, message);

        // Assert
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
        error.Type.Should().Be(expectedType);
    }

    [Fact]
    public void Given_None_Accessed_When_NoErrorExists_Then_ShouldReturnEmptyFailureError()
    {
        // Arrange
        var error = Error.None;

        // Act & Assert
        error.Code.Should().BeEmpty();
        error.Message.Should().BeEmpty();
        error.Type.Should().Be(ErrorType.Failure);
    }

    private static Error CreateError(string factoryName, string code, string message)
    {
        return factoryName switch
        {
            nameof(Error.Failure) => Error.Failure(code, message),
            nameof(Error.Validation) => Error.Validation(code, message),
            nameof(Error.NotFound) => Error.NotFound(code, message),
            nameof(Error.Conflict) => Error.Conflict(code, message),
            nameof(Error.Unauthorized) => Error.Unauthorized(code, message),
            _ => throw new InvalidOperationException("Invalid error factory.")
        };
    }
}
