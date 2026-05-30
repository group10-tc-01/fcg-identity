using Fcg.Identity.Domain.DonorProfiles;
using Fcg.Identity.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Identity.Infrastructure.SqlServer.Persistence.Configurations;

public sealed class DonorProfileConfiguration : IEntityTypeConfiguration<DonorProfile>
{
    public void Configure(EntityTypeBuilder<DonorProfile> builder)
    {
        builder.ToTable("DonorProfiles");

        builder.HasKey(donorProfile => donorProfile.Id);

        builder.Property(donorProfile => donorProfile.KeycloakUserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(donorProfile => donorProfile.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(donorProfile => donorProfile.Email)
            .HasConversion(email => email.Value, value => Email.Create(value).Value)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(donorProfile => donorProfile.Cpf)
            .HasConversion(cpf => cpf.Value, value => Cpf.Create(value).Value)
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(donorProfile => donorProfile.CreatedAt)
            .IsRequired();

        builder.HasIndex(donorProfile => donorProfile.KeycloakUserId)
            .IsUnique()
            .HasDatabaseName("UX_DonorProfiles_KeycloakUserId");

        builder.HasIndex(donorProfile => donorProfile.Email)
            .IsUnique()
            .HasDatabaseName("UX_DonorProfiles_Email");

        builder.HasIndex(donorProfile => donorProfile.Cpf)
            .IsUnique()
            .HasDatabaseName("UX_DonorProfiles_Cpf");
    }
}
