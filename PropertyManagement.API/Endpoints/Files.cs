using Carter;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        var group = app.MapGroup("/api/files").WithTags("Files");

        group.MapPost("", async ([FromForm] IFormFile file, ISender sender, CancellationToken ct) =>
        {
            if (file is null) return Results.BadRequest("File is required.");
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            var cmd = new CreateFileCommand(file.FileName, file.ContentType, ms.ToArray());
            var result = await sender.Send(cmd, ct);
            return result.IsValid ? Results.Created($"/api/files/{result.Value}", new { id = result.Value }) : ResultExtension.ResultToResponse(result);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetFileByIdQuery(id), ct);
            return result.IsValid ? Results.Ok(result.Value) : ResultExtension.ResultToResponse(result);
        })
        .Produces<FileResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteFileCommand(id), ct);
            return result.IsValid ? Results.NoContent() : ResultExtension.ResultToResponse(result);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
