using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.API.Contracts.Mappings;
using PropertyManagement.API.Contracts.Requests.Properties;
using PropertyManagement.API.Extensions;
using PropertyManagement.Application.Features.Properties.Create;
using PropertyManagement.Application.Features.Properties.Get;
using PropertyManagement.Application.Features.Properties.GetById;
using PropertyManagement.Shared.Pagination;

namespace PropertyManagement.API.Endpoints;

public sealed class Properties : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/properties").WithTags("Properties");

        group.MapGet("", async (
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromQuery] string? search,
            [FromQuery] int? statusId,
            [FromQuery] Guid? cityId,
            [FromQuery] string? orderBy,
            [FromQuery] bool desc,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetPropertiesQuery(
                PageNumber: pageNumber <= 0 ? 1 : pageNumber,
                PageSize: pageSize,
                Search: search,
                StatusId: statusId,
                CityId: cityId,
                OrderBy: orderBy,
                Desc: desc);

            var result = await sender.Send(query, ct);
            return result.IsValid ? Results.Ok(result.Value) : ResultExtension.ResultToResponse(result);
        })
        .Produces<PagedResult<PropertyListItem>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("", async ([FromBody] CreatePropertyRequest req, ISender sender, CancellationToken ct) =>
        {
            var command = req.ToCommand();
            var result = await sender.Send(command, ct);
            return result.IsValid ? Results.CreatedAtRoute(
                "GetProperty", 
                new { result.Value.Id}) : ResultExtension.ResultToResponse(result);
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
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetProperty");

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdatePropertyRequest req, ISender sender, CancellationToken ct) =>
        {
            var command = req.ToCommand(id);
            var result = await sender.Send(command, ct);
            return result.IsValid ? Results.NoContent() : ResultExtension.ResultToResponse(result);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
