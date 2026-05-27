using MediatR;
using Fcg.Identity.Domain;

namespace Fcg.Identity.Application.Abstractions.Messaging;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
