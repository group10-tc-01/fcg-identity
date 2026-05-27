using Fcg.Identity.Application.Abstractions.Messaging;

namespace Fcg.Identity.Application.UseCases.Items.CreateItem;

public sealed record CreateItemRequest(string Name, decimal Price) : ICommand<CreateItemResponse>;
