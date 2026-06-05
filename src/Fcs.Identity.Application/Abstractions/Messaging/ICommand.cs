using Fcs.Identity.Domain.Shared.Results;
using MediatR;

namespace Fcs.Identity.Application.Abstractions.Messaging;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
