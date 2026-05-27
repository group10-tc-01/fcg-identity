using Fcg.Identity.Application.Abstractions.Messaging;

namespace Fcg.Identity.Application.Messaging;

public sealed class NullMessagePublisher : IMessagePublisher
{
    public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
