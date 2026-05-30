using Fcg.Identity.Application.Messaging;
using FluentAssertions;

namespace Fcg.Identity.UnitTests.Application.Messaging;

public sealed class NullMessagePublisherTests
{
    [Fact]
    public async Task Given_PublishAsync_Called_When_MessageIsProvided_Then_ShouldCompleteSuccessfully()
    {
        // Arrange
        var publisher = new NullMessagePublisher();
        var message = new TestMessage("value");

        // Act
        var act = () => publisher.PublishAsync(message, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    private sealed record TestMessage(string Value);
}
