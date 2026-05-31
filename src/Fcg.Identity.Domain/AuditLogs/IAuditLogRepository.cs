namespace Fcg.Identity.Domain.AuditLogs;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
}
