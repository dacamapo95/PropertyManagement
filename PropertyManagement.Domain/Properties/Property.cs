using PropertyManagement.Domain.Countries;
using PropertyManagement.Domain.Owners;
using PropertyManagement.Shared.Primitives;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Domain.Properties;

public sealed class Property : AuditableEntity<Guid>
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public decimal Price { get; private set; }
    public int CodeInternal { get; set; }
    public int Year { get; set; }
    public int StatusId { get; private set; } = (int)PropertyStatusEnum.Draft;
    public PropertyStatus Status { get; set; } = default!;

    public Guid CityId { get; set; }
    public City City { get; set; } = default!;

    public Guid OwnerId { get; set; }
    public Owner Owner { get; set; } = default!;

    public ICollection<PropertyImage> Images { get; set; } = [];
    public ICollection<PropertyTrace> Traces { get; set; } = [];

    public Result ChangePrice(decimal newPrice, DateOnly changeDate, decimal tax)
    {
        if (newPrice <= 0)
            return Error.Validation("Price must be greater than zero.");

        if (tax < 0)
            return Error.Validation("Tax must be greater than or equal to zero.");

        var trace = PropertyTrace.Create(Id, name: "Price Update", value: newPrice, tax: tax, dateSale: changeDate);
        Price = newPrice;
        Traces.Add(trace);
        return Result.Success();
    }

    public Result SetInitialPrice(decimal initialPrice)
    {
        if (initialPrice <= 0)
            return Error.Validation("Price must be greater than zero.");

        Price = initialPrice;
        return Result.Success();
    }

    public Result SetStatus(PropertyStatusEnum newStatus)
    {
        if ((PropertyStatusEnum)StatusId == PropertyStatusEnum.Sold && newStatus != PropertyStatusEnum.Sold)
            return Error.Validation("Cannot change status after Sold.");

        StatusId = (int)newStatus;
        return Result.Success();
    }
}
