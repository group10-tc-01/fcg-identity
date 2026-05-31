using Fcg.Identity.Domain.AuditLogs;

namespace Fcg.Identity.CommomTestsUtilities.TestDoubles;

public sealed class InMemoryAuditLogRepository : IAuditLogRepository
{
    private readonly List<AuditLog> _auditLogs = [];

    public IReadOnlyCollection<AuditLog> AuditLogs => _auditLogs;

    public void Reset()
    {
        _auditLogs.Clear();
    }

    public Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        _auditLogs.Add(auditLog);
        return Task.CompletedTask;
    }
}
