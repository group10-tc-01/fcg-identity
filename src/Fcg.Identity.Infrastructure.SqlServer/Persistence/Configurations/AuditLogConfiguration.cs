using System.Diagnostics.CodeAnalysis;
using Fcg.Identity.Domain.AuditLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Identity.Infrastructure.SqlServer.Persistence.Configurations;

[ExcludeFromCodeCoverage]
public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(auditLog => auditLog.Id);

        builder.Property(auditLog => auditLog.ActorType)
            .HasMaxLength(50);

        builder.Property(auditLog => auditLog.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(auditLog => auditLog.EntityName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(auditLog => auditLog.EntityId)
            .HasMaxLength(100);

        builder.Property(auditLog => auditLog.OccurredAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(auditLog => auditLog.CorrelationId)
            .HasMaxLength(100);

        builder.Property(auditLog => auditLog.IpAddress)
            .HasMaxLength(45);

        builder.Property(auditLog => auditLog.UserAgent)
            .HasMaxLength(500);

        builder.Property(auditLog => auditLog.MetadataJson)
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(auditLog => new { auditLog.EntityName, auditLog.EntityId })
            .HasDatabaseName("IX_AuditLogs_EntityName_EntityId");

        builder.HasIndex(auditLog => auditLog.Action)
            .HasDatabaseName("IX_AuditLogs_Action");

        builder.HasIndex(auditLog => auditLog.OccurredAt)
            .HasDatabaseName("IX_AuditLogs_OccurredAt");

        builder.HasIndex(auditLog => auditLog.CorrelationId)
            .HasDatabaseName("IX_AuditLogs_CorrelationId");
    }
}
