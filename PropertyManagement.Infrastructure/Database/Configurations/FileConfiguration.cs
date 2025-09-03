using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainFile = PropertyManagement.Domain.Files.File;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class FileConfiguration : IEntityTypeConfiguration<DomainFile>
{
    public void Configure(EntityTypeBuilder<DomainFile> builder)
    {
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.OriginalName)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.MimeType)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Extension)
               .IsRequired()
               .HasMaxLength(10);

        builder.Property(x => x.Size)
               .IsRequired();

        builder.Property(x => x.Url)
               .HasMaxLength(1024);

        // Provider-specific binary column
        var provider = builder.Metadata.Model.GetProductVersion();
        builder.Property(x => x.Content)
               .HasColumnType("varbinary(max)");

        builder.HasMany(x => x.PropertyImages)
               .WithOne(pi => pi.File)
               .HasForeignKey(pi => pi.FileId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.OwnerImages)
               .WithOne(oi => oi.File)
               .HasForeignKey(oi => oi.FileId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
