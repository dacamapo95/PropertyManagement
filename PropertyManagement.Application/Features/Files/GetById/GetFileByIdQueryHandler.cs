using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Files;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Files.GetById;

public sealed class GetFileByIdQueryHandler(IFileRepository repo)
    : IQueryHandler<GetFileByIdQuery, FileResponse>
{
    private readonly IFileRepository _repo = repo;

    public async Task<Result<FileResponse>> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
    {
        _repo.DisableTracking();
        var entity = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Error.NotFound("File not found.");

        var base64 = entity.Content is null ? string.Empty : Convert.ToBase64String(entity.Content);
        var response = new FileResponse(
            entity.Id,
            entity.Name,
            entity.OriginalName,
            entity.MimeType,
            entity.Extension,
            entity.Size,
            entity.Url,
            base64);

        return response;
    }
}
