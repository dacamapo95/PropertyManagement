using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Domain.Properties;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class PropertyTraceConfiguration : IEntityTypeConfiguration<PropertyTrace>
{
    public void Configure(EntityTypeBuilder<PropertyTrace> builder)
    {
        builder.Property(x => x.PropertyId).IsRequired();
        builder.Property(x => x.DateSale).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Value).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.Tax).IsRequired().HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Property)
               .WithMany(p => p.Traces)
               .HasForeignKey(x => x.PropertyId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
