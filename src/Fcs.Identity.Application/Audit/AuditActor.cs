namespace Fcs.Identity.Application.Audit;

public sealed record AuditActor(Guid? ActorId, string? ActorType);
