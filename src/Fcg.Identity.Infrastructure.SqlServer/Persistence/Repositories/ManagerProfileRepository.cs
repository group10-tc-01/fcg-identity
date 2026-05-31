using Fcg.Identity.Domain.ManagerProfiles;
using Fcg.Identity.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Identity.Infrastructure.SqlServer.Persistence.Repositories;

public sealed class ManagerProfileRepository : IManagerProfileRepository
{
    private readonly FcgIdentityDbContext _dbContext;

    public ManagerProfileRepository(FcgIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(ManagerProfile managerProfile, CancellationToken cancellationToken = default)
    {
        return _dbContext.ManagerProfiles.AddAsync(managerProfile, cancellationToken).AsTask();
    }

    public Task<ManagerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.ManagerProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(managerProfile => managerProfile.Id == id, cancellationToken);
    }

    public Task<ManagerProfile?> GetByKeycloakUserIdAsync(string keycloakUserId, CancellationToken cancellationToken = default)
    {
        return _dbContext.ManagerProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(managerProfile => managerProfile.KeycloakUserId == keycloakUserId, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
        {
            return Task.FromResult(false);
        }

        return _dbContext.ManagerProfiles.AnyAsync(managerProfile => managerProfile.Email == emailResult.Value, cancellationToken);
    }
}
