using PropertyManagement.API.Contracts.Requests.Properties;
using PropertyManagement.Application.Features.Properties.Create;
using PropertyManagement.Application.Features.Properties.Update;

namespace PropertyManagement.API.Contracts.Mappings;

public static class PropertyRequestMappings
{
    public static OwnerCommand ToCommand(this OwnerRequest req)
        => new(req.IdentificationTypeId, req.IdentificationNumber, req.Name, req.Address, req.BirthDate, req.OwnerFileIds);

    public static CreatePropertyCommand ToCommand(this CreatePropertyRequest req)
        => new(req.Name, req.Address, req.Price, req.CodeInternal, req.Year, req.StatusId, req.CityId, req.Owner.ToCommand(), req.PropertyFileIds);

    public static OwnerUpdate ToUpdate(this OwnerRequest req)
        => new(req.IdentificationTypeId, req.IdentificationNumber, req.Name, req.Address, req.BirthDate, req.OwnerFileIds);

    public static UpdatePropertyCommand ToCommand(this UpdatePropertyRequest req, Guid id)
        => new(id, req.Name, req.Address, req.CodeInternal, req.Year, req.CityId, req.StatusId, req.Price, req.Tax, req.PriceDate, req.Owner.ToUpdate(), req.PropertyFileIds);
}
