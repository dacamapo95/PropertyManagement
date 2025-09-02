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
    public Guid CountryId { get; set; }
    public Country Country { get; set; } = default!;
    public Guid StateId { get; set; }
    public State State { get; set; } = default!;
    public Guid CityId { get; set; }
    public City City { get; set; } = default!;
    public Guid OwnerId { get; set; }
    public Owner Owner { get; set; } = default!;

    public ICollection<PropertyImage> Images { get; set; } = [];
    public ICollection<PropertyTrace> Traces { get; set; } = [];

    public Result ChangePrice(decimal newPrice, DateOnly changeDate, string changedBy)
    {
        if (newPrice <= 0)
            return Result.Fail(Error.Validation("Price must be greater than zero."));

        var trace = PropertyTrace.Create(Id, name: "Price Update", value: newPrice, tax: 0m, dateSale: changeDate, createdBy: changedBy);
        Price = newPrice;
        Traces.Add(trace);
        return Result.Success();
    }

    public Result SetStatus(PropertyStatusEnum newStatus, string changedBy)
    {
        if ((PropertyStatusEnum)StatusId == PropertyStatusEnum.Sold && newStatus != PropertyStatusEnum.Sold)
            return Result.Fail(Error.Validation("Cannot change status after Sold."));

        StatusId = (int)newStatus;
        return Result.Success();
    }
}
