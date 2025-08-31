using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Domain.Countries;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.CurrencyCode).HasMaxLength(3);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(x => x.States)
               .WithOne(d => d.Country)
               .HasForeignKey(d => d.CountryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
