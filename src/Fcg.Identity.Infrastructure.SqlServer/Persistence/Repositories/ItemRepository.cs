using Fcg.Identity.Domain.Items;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Identity.Infrastructure.SqlServer.Persistence.Repositories;

public sealed class ItemRepository : IItemRepository
{
    private readonly FcgIdentityDbContext _dbContext;

    public ItemRepository(FcgIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Item item, CancellationToken cancellationToken = default)
    {
        return _dbContext.Items.AddAsync(item, cancellationToken).AsTask();
    }

    public Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Items.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbContext.Items.AnyAsync(item => item.Name == name, cancellationToken);
    }
}
