using Fcg.Identity.Domain.DonorProfiles;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Identity.Infrastructure.SqlServer.Persistence.Repositories;

public sealed class DonorProfileRepository : IDonorProfileRepository
{
    private readonly FcgIdentityDbContext _dbContext;

    public DonorProfileRepository(FcgIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(DonorProfile donorProfile, CancellationToken cancellationToken = default)
    {
        return _dbContext.DonorProfiles.AddAsync(donorProfile, cancellationToken).AsTask();
    }

    public Task<DonorProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.DonorProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(donorProfile => donorProfile.Id == id, cancellationToken);
    }

    public Task<DonorProfile?> GetByKeycloakUserIdAsync(string keycloakUserId, CancellationToken cancellationToken = default)
    {
        return _dbContext.DonorProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(donorProfile => donorProfile.KeycloakUserId == keycloakUserId, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return _dbContext.DonorProfiles.AnyAsync(donorProfile => donorProfile.Email == normalizedEmail, cancellationToken);
    }

    public Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var normalizedCpf = cpf.Trim();
        return _dbContext.DonorProfiles.AnyAsync(donorProfile => donorProfile.Cpf == normalizedCpf, cancellationToken);
    }
}
