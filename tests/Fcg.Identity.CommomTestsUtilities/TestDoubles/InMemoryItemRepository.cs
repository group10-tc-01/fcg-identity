using Fcg.Identity.Domain.Items;

namespace Fcg.Identity.CommomTestsUtilities.TestDoubles;

public sealed class InMemoryItemRepository : IItemRepository
{
    private readonly Dictionary<Guid, Item> _items = new();

    public Task AddAsync(Item item, CancellationToken cancellationToken = default)
    {
        _items[item.Id] = item;
        return Task.CompletedTask;
    }

    public Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _items.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var exists = _items.Values.Any(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }
}
