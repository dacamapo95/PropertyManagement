using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Domain.Owners;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Address)
               .HasMaxLength(400);

        builder.Property(x => x.IdentificationNumber)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(x => new { x.IdentificationTypeId, x.IdentificationNumber })
               .IsUnique();

        builder.HasOne(x => x.IdentificationType)
               .WithMany(x => x.Owners)
               .HasForeignKey(x => x.IdentificationTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.OwnerImages)
               .WithOne(oi => oi.Owner)
               .HasForeignKey(oi => oi.OwnerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
