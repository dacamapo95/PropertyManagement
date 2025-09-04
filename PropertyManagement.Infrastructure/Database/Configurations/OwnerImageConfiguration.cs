using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Domain.Owners;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class OwnerImageConfiguration : IEntityTypeConfiguration<OwnerImage>
{
    public void Configure(EntityTypeBuilder<OwnerImage> builder)
    {
        builder.ToTable("OwnerImages");

        builder.HasKey(x => new { x.OwnerId, x.FileId });

        builder.Property(x => x.OwnerId).IsRequired();
        builder.Property(x => x.FileId).IsRequired();

        builder.HasOne(x => x.Owner)
               .WithMany(o => o.OwnerImages)
               .HasForeignKey(x => x.OwnerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.File)
               .WithMany(f => f.OwnerImages)
               .HasForeignKey(x => x.FileId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
