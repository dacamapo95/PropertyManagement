using Carter;
using MediatR;
using PropertyManagement.Application.Features.Countries.Get;
using PropertyManagement.Application.Features.Countries.GetStates;
using PropertyManagement.Application.Features.Countries.GetCities;

namespace PropertyManagement.API.Endpoints;

public sealed class Countries : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group =  app.MapGroup("/api/countries").WithTags("Countries");

        group.MapGet("", async (string? search, ISender sender) =>
        {
            var result = await sender.Send(new GetCountriesQuery());
            return Results.Ok(result.Value);
        });

        group.MapGet("/{countryId:guid}/states", async (Guid countryId, ISender sender) =>
        {
            var result = await sender.Send(new GetStatesByCountryIdQuery(countryId));
            return Results.Ok(result.Value);
        });

        group.MapGet("/states/{stateId:guid}/cities", async (Guid stateId, ISender sender) =>
        {
            var result = await sender.Send(new GetCitiesByStateIdQuery(stateId));
            return Results.Ok(result.Value);
        });
        
    }
}
