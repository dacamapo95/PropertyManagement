using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Files;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Files.Delete;

public sealed class DeleteFileCommandHandler(IFileRepository repo, IUnitOfWork uow)
    : ICommandHandler<DeleteFileCommand, bool>
{
    private readonly IFileRepository _repo = repo;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result<bool>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var file = await _repo.GetByIdAsync(request.FileId, cancellationToken);
        if (file is null) return Error.NotFound("File not found.");

        _repo.Remove(file);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
