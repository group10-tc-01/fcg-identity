using System.Diagnostics.CodeAnalysis;
using Fcs.Identity.Domain.ManagerProfiles;
using Fcs.Identity.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcs.Identity.Infrastructure.SqlServer.Persistence.Configurations;

[ExcludeFromCodeCoverage]
public sealed class ManagerProfileConfiguration : BaseConfiguration<ManagerProfile>
{
    public override void Configure(EntityTypeBuilder<ManagerProfile> builder)
    {
        base.Configure(builder);

        builder.ToTable("ManagerProfiles");

        builder.Property(managerProfile => managerProfile.KeycloakUserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(managerProfile => managerProfile.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(managerProfile => managerProfile.Email)
            .HasConversion(email => email.Value, value => Email.Create(value).Value)
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(managerProfile => managerProfile.KeycloakUserId)
            .IsUnique()
            .HasDatabaseName("UX_ManagerProfiles_KeycloakUserId");

        builder.HasIndex(managerProfile => managerProfile.Email)
            .IsUnique()
            .HasDatabaseName("UX_ManagerProfiles_Email");
    }
}
