using Fcs.Identity.Domain.Shared.Results;
using MediatR;

namespace Fcs.Identity.Application.Abstractions.Messaging;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
