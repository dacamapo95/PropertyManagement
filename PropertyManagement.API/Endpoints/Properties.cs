using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.API.Extensions;
using PropertyManagement.Application.Features.Properties.Create;
using PropertyManagement.Application.Features.Properties.GetById;
using PropertyManagement.Application.Features.Properties.Update;

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
    OwnerDto Owner,
    IReadOnlyList<Guid> PropertyFileIds
);

public sealed record OwnerDto(
    int IdentificationTypeId,
    string IdentificationNumber,
    string Name,
    string? Address,
    DateOnly? BirthDate,
    IReadOnlyList<Guid> OwnerFileIds
);

public sealed record UpdatePropertyRequest(
    string Name,
    string Address,
    int CodeInternal,
    int Year,
    Guid CountryId,
    Guid StateId,
    Guid CityId,
    int StatusId,
    decimal? Price,
    DateOnly? PriceDate,
    OwnerDto Owner,
    IReadOnlyList<Guid> PropertyFileIds
);

public sealed class Properties : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/properties").WithTags("Properties");

        group.MapPost("", async ([FromBody] CreatePropertyRequest req, ISender sender, CancellationToken ct) =>
        {
            var ownerInput = new PropertyManagement.Application.Features.Properties.Create.OwnerCommand(
                req.Owner.IdentificationTypeId, req.Owner.IdentificationNumber, req.Owner.Name, req.Owner.Address, req.Owner.BirthDate, req.Owner.OwnerFileIds);

            var cmd = new CreatePropertyCommand(
                req.Name, req.Address, req.Price, req.CodeInternal, req.Year, req.StatusId,
                req.CountryId, req.StateId, req.CityId,
                ownerInput,
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

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdatePropertyRequest req, ISender sender, CancellationToken ct) =>
        {
            var ownerUpdate = new OwnerUpdate(
                req.Owner.IdentificationTypeId,
                req.Owner.IdentificationNumber,
                req.Owner.Name,
                req.Owner.Address,
                req.Owner.BirthDate,
                req.Owner.OwnerFileIds);

            var cmd = new UpdatePropertyCommand(
                id,
                req.Name,
                req.Address,
                req.CodeInternal,
                req.Year,
                req.CountryId,
                req.StateId,
                req.CityId,
                req.StatusId,
                req.Price,
                req.PriceDate,
                ownerUpdate,
                req.PropertyFileIds);

            var result = await sender.Send(cmd, ct);
            return result.IsValid ? Results.NoContent() : ResultExtension.ResultToResponse(result);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
