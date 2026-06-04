using Fcg.Identity.Domain.ManagerProfiles;

namespace Fcg.Identity.CommomTestsUtilities.TestDoubles;

public sealed class InMemoryManagerProfileRepository : IManagerProfileRepository
{
    private readonly List<ManagerProfile> _managerProfiles = [];

    public IReadOnlyCollection<ManagerProfile> ManagerProfiles => _managerProfiles;

    public void Reset()
    {
        _managerProfiles.Clear();
    }

    public Task AddAsync(ManagerProfile managerProfile, CancellationToken cancellationToken = default)
    {
        _managerProfiles.Add(managerProfile);
        return Task.CompletedTask;
    }

    public Task<ManagerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_managerProfiles.FirstOrDefault(managerProfile => managerProfile.Id == id));
    }

    public Task<ManagerProfile?> GetByKeycloakUserIdAsync(string keycloakUserId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_managerProfiles.FirstOrDefault(managerProfile => managerProfile.KeycloakUserId == keycloakUserId));
    }

    public Task<ManagerProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_managerProfiles.FirstOrDefault(managerProfile => managerProfile.Email.Value == email));
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_managerProfiles.Any(managerProfile => managerProfile.Email.Value == email));
    }

    public void Update(ManagerProfile managerProfile)
    {
        var index = _managerProfiles.FindIndex(existing => existing.Id == managerProfile.Id);
        if (index >= 0)
        {
            _managerProfiles[index] = managerProfile;
        }
    }
}
