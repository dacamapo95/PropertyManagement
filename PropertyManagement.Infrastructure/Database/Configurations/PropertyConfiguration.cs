using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Domain.Properties;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Address).IsRequired().HasMaxLength(400);
        builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.CodeInternal).IsRequired();
        builder.Property(x => x.Year).IsRequired();
        builder.Property(x => x.StatusId).IsRequired();

        builder.HasOne(x => x.Status)
               .WithMany(x => x.Properties)
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Country)
               .WithMany(x => x.Properties)
               .HasForeignKey(x => x.CountryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.State)
               .WithMany(x => x.Properties)
               .HasForeignKey(x => x.StateId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.City)
               .WithMany(x => x.Properties)
               .HasForeignKey(x => x.CityId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Owner)
               .WithMany(o => o.Properties)
               .HasForeignKey(x => x.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Price);
        builder.HasIndex(x => x.StatusId);
        builder.HasIndex(x => new { x.CountryId, x.StateId, x.CityId });
    }
}
