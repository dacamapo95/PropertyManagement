using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Domain.Properties;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.HasKey(x => new { x.PropertyId, x.FileId });

        builder.HasOne(x => x.Property)
               .WithMany(p => p.Images)
               .HasForeignKey(x => x.PropertyId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.File)
               .WithMany()
               .HasForeignKey(x => x.FileId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
