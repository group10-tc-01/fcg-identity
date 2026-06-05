using Fcs.Identity.Domain.Abstractions;

namespace Fcs.Identity.CommomTestsUtilities.TestDoubles;

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCalls { get; private set; }

    public void Reset()
    {
        SaveChangesCalls = 0;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCalls++;
        return Task.FromResult(1);
    }
}
