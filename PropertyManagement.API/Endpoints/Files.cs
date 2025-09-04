using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.API.Extensions;
using PropertyManagement.Application.Features.Files.Create;
using PropertyManagement.Application.Features.Files.Delete;
using PropertyManagement.Application.Features.Files.GetById;

namespace PropertyManagement.API.Endpoints;

public sealed class Files : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/files").WithTags("Files").WithOpenApi().RequireAuthorization();

        group.MapPost("", async ([FromForm] IFormFile file, ISender sender, CancellationToken ct) =>
        {
            if (file is null) return Results.BadRequest("File is required.");
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            var result = await sender.Send(new CreateFileCommand(file.FileName, file.ContentType, ms.ToArray()), ct);
            return result.IsValid ? Results.CreatedAtRoute("DeleteFile",
                 new { id = result.Value }) : ResultExtension.ResultToResponse(result);
        })
        .WithSummary("Subir archivo al sistema")
        .WithDescription("Permite cargar archivos como fotos de propiedades, documentos del propietario, planos, etc. Soporta validación de tipo y tamaño según configuración.")
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetFileByIdQuery(id), ct);
            return result.IsValid ? Results.Ok(result.Value) : ResultExtension.ResultToResponse(result);
        })
        .WithSummary("Descargar archivo por ID")
        .WithDescription("Obtiene un archivo específico con sus metadatos y contenido en base64.")
        .Produces<FileResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("DeleteFile");

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteFileCommand(id), ct);
            return result.IsValid ? Results.NoContent() : ResultExtension.ResultToResponse(result);
        })
        .WithSummary("Eliminar archivo del sistema")
        .WithDescription("Borra permanentemente un archivo del sistema.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
