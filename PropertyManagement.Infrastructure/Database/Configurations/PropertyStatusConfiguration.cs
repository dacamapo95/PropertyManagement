using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Domain.Properties;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class PropertyStatusConfiguration : IEntityTypeConfiguration<PropertyStatus>
{
    public void Configure(EntityTypeBuilder<PropertyStatus> builder)
    {
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);

    }
}
