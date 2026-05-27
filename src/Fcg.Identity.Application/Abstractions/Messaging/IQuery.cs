using MediatR;
using Fcg.Identity.Domain;

namespace Fcg.Identity.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
