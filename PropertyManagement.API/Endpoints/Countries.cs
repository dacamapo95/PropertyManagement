using Carter;
using MediatR;
using PropertyManagement.Application.Features.Countries.Get;
using PropertyManagement.Application.Features.Countries.GetStates;
using PropertyManagement.Application.Features.Countries.GetCities;
using PropertyManagement.API.Extensions;

namespace PropertyManagement.API.Endpoints;

public sealed class Countries : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group =  app.MapGroup("/api/countries").WithTags("Countries")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("", async (ISender sender) =>
        {
            var result = await sender.Send(new GetCountriesQuery());
            return result.IsValid ? Results.Ok(result.Value) : ResultExtension.ResultToResponse(result);
        })
        .WithSummary("Obtener lista de países")
        .WithDescription("Consulta todos los países disponibles en el sistema. Utilizado para seleccionar ubicación de propiedades en formularios.")
        .Produces<IReadOnlyList<CountryResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{countryId:guid}/states", async (Guid countryId, ISender sender) =>
        {
            var result = await sender.Send(new GetStatesByCountryIdQuery(countryId));
            return result.IsValid ? Results.Ok(result.Value) : ResultExtension.ResultToResponse(result);
        })
        .WithSummary("Obtener departamentos/estados por país")
        .WithDescription("Consulta los departamentos o estados de un país específico. Esencial para ubicación jerárquica de propiedades (país > departamento > ciudad).")
        .Produces<IReadOnlyList<StateResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/states/{stateId:guid}/cities", async (Guid stateId, ISender sender) =>
        {
            var result = await sender.Send(new GetCitiesByStateIdQuery(stateId));
            return result.IsValid ? Results.Ok(result.Value) : ResultExtension.ResultToResponse(result);
        })
        .WithSummary("Obtener ciudades por departamento/estado")
        .WithDescription("Consulta las ciudades de un departamento o estado específico. Nivel final en la jerarquía de ubicación para registro de propiedades.")
        .Produces<IReadOnlyList<CityResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
        
    }
}
