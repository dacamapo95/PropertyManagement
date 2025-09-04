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
        var group = app.MapGroup("/api/properties")
            .WithTags("Properties")
            .RequireAuthorization()
            .WithOpenApi();

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
        .WithSummary("Obtener lista paginada de propiedades")
        .WithDescription("Consulta propiedades con paginación, filtros por nombre/dirección, estado, ciudad y ordenamiento personalizable. Ideal para listados y búsquedas.")
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
        .WithSummary("Crear nueva propiedad")
        .WithDescription("Registra una nueva propiedad en el sistema incluyendo datos del propietario, precio, ubicación y archivos asociados. El propietario se crea o actualiza automáticamente.")
        .Produces<CreatePropertyResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPropertyByIdQuery(id), ct);
            return result.IsValid ? Results.Ok(result.Value) : ResultExtension.ResultToResponse(result);
        })
        .WithSummary("Obtener propiedad por ID")
        .WithDescription("Consulta los detalles completos de una propiedad específica incluyendo información del propietario, ciudad, estado e imágenes asociadas.")
        .Produces<PropertyResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetProperty");

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdatePropertyRequest req, ISender sender, CancellationToken ct) =>
        {
            var command = req.ToCommand(id);
            var result = await sender.Send(command, ct);
            return result.IsValid ? Results.NoContent() : ResultExtension.ResultToResponse(result);
        })
        .WithSummary("Actualizar propiedad existente")
        .WithDescription("Modifica los datos de una propiedad incluyendo precio (con auditoría automática), información del propietario y archivos. Actualiza fecha de modificación automáticamente.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
