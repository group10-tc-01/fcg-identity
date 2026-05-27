namespace Fcg.Identity.Domain.Items;

public interface IItemRepository
{
    Task AddAsync(Item item, CancellationToken cancellationToken = default);
    Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}
