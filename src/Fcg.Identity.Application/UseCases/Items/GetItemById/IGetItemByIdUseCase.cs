using Fcg.Identity.Application.Abstractions.Messaging;

namespace Fcg.Identity.Application.UseCases.Items.GetItemById;

public interface IGetItemByIdUseCase : IQueryHandler<GetItemByIdRequest, GetItemByIdResponse>
{
}
