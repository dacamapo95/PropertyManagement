using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Domain.Countries;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class StateConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

        builder.HasOne(x => x.Country)
               .WithMany(c => c.States)
               .HasForeignKey(x => x.CountryId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Cities)
               .WithOne(c => c.State)
               .HasForeignKey(c => c.StateId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
