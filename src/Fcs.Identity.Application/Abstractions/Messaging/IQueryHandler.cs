using Fcs.Identity.Domain.Shared.Results;
using MediatR;

namespace Fcs.Identity.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
