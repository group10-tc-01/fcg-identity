using Fcg.Identity.Domain.Items;
using Fcg.Identity.Domain.Results;
using Fcg.Identity.Messages;

namespace Fcg.Identity.Application.UseCases.Items.GetItemById;

public sealed class GetItemByIdUseCase : IGetItemByIdUseCase
{
    private readonly IItemRepository _itemRepository;

    public GetItemByIdUseCase(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Result<GetItemByIdResponse>> Handle(GetItemByIdRequest request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            return Error.NotFound("Item.NotFound", ResourceMessages.ItemNotFound);
        }

        return new GetItemByIdResponse(item.Id, item.Name, item.Price, item.CreatedAt);
    }
}
