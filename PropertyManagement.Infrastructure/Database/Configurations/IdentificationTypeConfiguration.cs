using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropertyManagement.Domain.Owners;

namespace PropertyManagement.Infrastructure.Database.Configurations;

public sealed class IdentificationTypeConfiguration : IEntityTypeConfiguration<IdentificationType>
{
    public void Configure(EntityTypeBuilder<IdentificationType> builder)
    {
        builder.Property(x => x.Id)
       .ValueGeneratedNever();

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);
    }
}
