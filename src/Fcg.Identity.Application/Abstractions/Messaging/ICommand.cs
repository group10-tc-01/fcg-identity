using Fcg.Identity.Domain.Results;
using MediatR;

namespace Fcg.Identity.Application.Abstractions.Messaging;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
