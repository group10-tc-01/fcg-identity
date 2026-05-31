using Fcg.Identity.Domain.AuditLogs;

namespace Fcg.Identity.Infrastructure.SqlServer.Persistence.Repositories;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly FcgIdentityDbContext _dbContext;

    public AuditLogRepository(FcgIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        return _dbContext.AuditLogs.AddAsync(auditLog, cancellationToken).AsTask();
    }
}
