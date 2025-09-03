using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Domain.Properties;

public sealed class PropertyTrace : AuditableEntity<Guid>
{
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = default!;
    public DateOnly DateSale { get; set; }
    public string Name { get; set; } = default!;
    public decimal Value { get; set; }
    public decimal Tax { get; set; }

    public static PropertyTrace Create(Guid propertyId, string name, decimal value, decimal tax, DateOnly dateSale)
    {
        return new PropertyTrace
        {
            Id = Guid.NewGuid(),
            PropertyId = propertyId,
            Name = name,
            Value = value,
            Tax = tax,
            DateSale = dateSale,
        };
    }
}
