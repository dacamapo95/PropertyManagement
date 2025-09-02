using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Domain.Owners;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class OwnerImageConfiguration : IEntityTypeConfiguration<OwnerImage>
{
    public void Configure(EntityTypeBuilder<OwnerImage> builder)
    {
        builder.HasKey(x => new { x.OwnerId, x.FileId });

        builder.HasOne(x => x.Owner)
               .WithMany(o => o.OwnerImages)
               .HasForeignKey(x => x.OwnerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.File)
               .WithMany()
               .HasForeignKey(x => x.FileId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
