using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.API.Extensions;
using PropertyManagement.Application.Features.Properties.Create;
using PropertyManagement.Application.Features.Properties.GetById;

namespace PropertyManagement.API.Endpoints;

public sealed record CreatePropertyRequest(
    string Name,
    string Address,
    decimal Price,
    int CodeInternal,
    int Year,
    int StatusId,
    Guid CountryId,
    Guid StateId,
    Guid CityId,
    OwnerRequest Owner,
    IReadOnlyList<Guid> PropertyFileIds
);

public sealed record OwnerRequest(
    int IdentificationTypeId,
    string IdentificationNumber,
    string Name,
    string? Address,
    DateOnly? BirthDate,
    IReadOnlyList<Guid> OwnerFileIds
);

public sealed class Properties : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/properties").WithTags("Properties");

        group.MapPost("", async ([FromBody] CreatePropertyRequest req, ISender sender, CancellationToken ct) =>
        {
            var cmd = new CreatePropertyCommand(
                req.Name, req.Address, req.Price, req.CodeInternal, req.Year, req.StatusId,
                req.CountryId, req.StateId, req.CityId,
                new OwnerCommand(req.Owner.IdentificationTypeId, req.Owner.IdentificationNumber, req.Owner.Name, req.Owner.Address, req.Owner.BirthDate, req.Owner.OwnerFileIds),
                req.PropertyFileIds);

            var result = await sender.Send(cmd, ct);
            return result.IsValid ? Results.Created($"/api/properties/{result.Value.Id}", result.Value) : ResultExtension.ResultToResponse(result);
        })
        .Produces<CreatePropertyResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPropertyByIdQuery(id), ct);
            return result.IsValid ? Results.Ok(result.Value) : ResultExtension.ResultToResponse(result);
        })
        .Produces<PropertyResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
