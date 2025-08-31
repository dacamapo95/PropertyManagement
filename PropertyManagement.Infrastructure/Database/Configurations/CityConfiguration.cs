using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Domain.Countries;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

        builder.HasOne(x => x.State)
               .WithMany(s => s.Cities)
               .HasForeignKey(x => x.StateId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
