namespace Fcg.Identity.Domain.ManagerProfiles;

public interface IManagerProfileRepository
{
    Task AddAsync(ManagerProfile managerProfile, CancellationToken cancellationToken = default);

    Task<ManagerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ManagerProfile?> GetByKeycloakUserIdAsync(string keycloakUserId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
