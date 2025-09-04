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
        var group = app.MapGroup("/api/files").WithTags("Files").RequireAuthorization();

        group.MapPost("", async ([FromForm] IFormFile file, ISender sender, CancellationToken ct) =>
        {
            if (file is null) return Results.BadRequest("File is required.");
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            var result = await sender.Send(new CreateFileCommand(file.FileName, file.ContentType, ms.ToArray()), ct);
            return result.IsValid ? Results.CreatedAtRoute("DeleteFile",
                 new { id = result.Value }) : ResultExtension.ResultToResponse(result);
        })
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetFileByIdQuery(id), ct);
            return result.IsValid ? Results.Ok(result.Value) : ResultExtension.ResultToResponse(result);
        })
        .Produces<FileResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("DeleteFile");

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteFileCommand(id), ct);
            return result.IsValid ? Results.NoContent() : ResultExtension.ResultToResponse(result);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
