using Fcg.Identity.Domain.Results;
using MediatR;

namespace Fcg.Identity.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
