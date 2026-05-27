using Fcg.Identity.Application.Abstractions.Messaging;

namespace Fcg.Identity.Application.UseCases.Items.CreateItem;

public interface ICreateItemUseCase : ICommandHandler<CreateItemRequest, CreateItemResponse>
{
}
