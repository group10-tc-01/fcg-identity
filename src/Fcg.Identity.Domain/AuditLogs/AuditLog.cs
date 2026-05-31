using Fcg.Identity.Domain.Shared.Results;

namespace Fcg.Identity.Domain.AuditLogs;

public sealed class AuditLog
{
    private AuditLog()
    {
    }

    private AuditLog(
        Guid id,
        Guid? actorId,
        string? actorType,
        string action,
        string entityName,
        string? entityId,
        DateTime occurredAt,
        string? correlationId,
        string? ipAddress,
        string? userAgent,
        string? metadataJson)
    {
        Id = id;
        ActorId = actorId;
        ActorType = actorType;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        OccurredAt = occurredAt;
        CorrelationId = correlationId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        MetadataJson = metadataJson;
    }

    public Guid Id { get; private set; }
    public Guid? ActorId { get; private set; }
    public string? ActorType { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public string? CorrelationId { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? MetadataJson { get; private set; }

    public static Result<AuditLog> Create(
        string action,
        string entityName,
        Guid? actorId = null,
        string? actorType = null,
        string? entityId = null,
        string? correlationId = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? metadataJson = null)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return Error.Validation("AuditLog.ActionRequired", "Action is required.");
        }

        if (string.IsNullOrWhiteSpace(entityName))
        {
            return Error.Validation("AuditLog.EntityNameRequired", "Entity name is required.");
        }

        return new AuditLog(
            Guid.NewGuid(),
            actorId,
            actorType,
            action,
            entityName,
            entityId,
            DateTime.UtcNow,
            correlationId,
            ipAddress,
            userAgent,
            metadataJson);
    }
}
