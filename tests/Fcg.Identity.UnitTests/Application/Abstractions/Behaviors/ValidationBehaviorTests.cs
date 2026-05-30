using Fcg.Identity.Application.Abstractions.Behaviors;
using Fcg.Identity.Domain.Shared.Results;
using FluentAssertions;
using FluentValidation;
using MediatR;

namespace Fcg.Identity.UnitTests.Application.Abstractions.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Given_Handle_Called_When_NoValidatorsExist_Then_ShouldCallNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestRequest, Result<TestResponse>>([]);
        var nextCalls = 0;

        // Act
        var result = await behavior.Handle(
            new TestRequest("value"),
            cancellationToken =>
            {
                nextCalls++;
                return Task.FromResult<Result<TestResponse>>(new TestResponse("ok"));
            },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("ok");
        nextCalls.Should().Be(1);
    }

    [Fact]
    public async Task Given_Handle_Called_When_RequestIsValid_Then_ShouldCallNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestRequest, Result<TestResponse>>([new TestRequestValidator()]);
        var nextCalls = 0;

        // Act
        var result = await behavior.Handle(
            new TestRequest("value"),
            cancellationToken =>
            {
                nextCalls++;
                return Task.FromResult<Result<TestResponse>>(new TestResponse("ok"));
            },
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("ok");
        nextCalls.Should().Be(1);
    }

    [Fact]
    public async Task Given_Handle_Called_When_RequestIsInvalid_Then_ShouldReturnValidationFailure()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestRequest, Result<TestResponse>>([new TestRequestValidator()]);
        var nextCalls = 0;

        // Act
        var result = await behavior.Handle(
            new TestRequest(string.Empty),
            cancellationToken =>
            {
                nextCalls++;
                return Task.FromResult<Result<TestResponse>>(new TestResponse("ok"));
            },
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Failed");
        result.Error.Message.Should().Be("Value is required.");
        nextCalls.Should().Be(0);
    }

    [Fact]
    public async Task Given_Handle_Called_When_ResponseIsNotResult_Then_ShouldThrowValidationException()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestRequest, TestResponse>([new TestRequestValidator()]);

        // Act
        var act = () => behavior.Handle(
            new TestRequest(string.Empty),
            cancellationToken => Task.FromResult(new TestResponse("ok")),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Value is required.");
    }

    private sealed record TestRequest(string Value) : IRequest<Result<TestResponse>>;

    private sealed record TestResponse(string Value);

    private sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(request => request.Value)
                .NotEmpty()
                .WithMessage("Value is required.");
        }
    }
}
