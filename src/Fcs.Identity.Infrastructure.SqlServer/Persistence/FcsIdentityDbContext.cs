using System.Diagnostics.CodeAnalysis;
using Fcs.Identity.Domain.Abstractions;
using Fcs.Identity.Domain.DonorProfiles;
using Fcs.Identity.Domain.ManagerProfiles;
using Microsoft.EntityFrameworkCore;

namespace Fcs.Identity.Infrastructure.SqlServer.Persistence;

[ExcludeFromCodeCoverage]
public sealed class FcsIdentityDbContext : DbContext, IUnitOfWork
{
    public FcsIdentityDbContext(DbContextOptions<FcsIdentityDbContext> options) : base(options)
    {
    }

    public DbSet<DonorProfile> DonorProfiles => Set<DonorProfile>();
    public DbSet<ManagerProfile> ManagerProfiles => Set<ManagerProfile>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FcsIdentityDbContext).Assembly);
    }
}
