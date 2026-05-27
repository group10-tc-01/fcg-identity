namespace Fcg.Identity.Application.UseCases.Items.CreateItem;

public sealed record CreateItemResponse(Guid Id, string Name, decimal Price, DateTime CreatedAt);
