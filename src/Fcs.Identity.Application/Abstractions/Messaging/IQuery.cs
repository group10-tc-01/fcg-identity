using Fcs.Identity.Domain.Shared.Results;
using MediatR;

namespace Fcs.Identity.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
