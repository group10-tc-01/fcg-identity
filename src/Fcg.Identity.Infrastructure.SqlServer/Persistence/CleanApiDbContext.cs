using Fcg.Identity.Domain.Abstractions;
using Fcg.Identity.Domain.Items;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Identity.Infrastructure.SqlServer.Persistence;

public sealed class CleanApiDbContext : DbContext, IUnitOfWork
{
    public CleanApiDbContext(DbContextOptions<CleanApiDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items => Set<Item>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CleanApiDbContext).Assembly);
    }
}
