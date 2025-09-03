using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Owners;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Properties.Update;

public sealed class UpdatePropertyCommandHandler(
    IPropertyRepository repo,
    Domain.Files.IFileRepository files,
    IOwnerRepository owners,
    IUnitOfWork uow)
    : ICommandHandler<UpdatePropertyCommand>
{
    private readonly IPropertyRepository _repo = repo;
    private readonly Domain.Files.IFileRepository _files = files;
    private readonly IOwnerRepository _owners = owners;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _repo.GetDetailsByIdAsync(request.Id, cancellationToken);
        if (property is null) return Error.NotFound("Property not found.");

        var filesOk = await ValidateFilesExistAsync(request.PropertyFileIds, cancellationToken);
        if (filesOk.IsFailure) return filesOk.Error;

        filesOk = await ValidateFilesExistAsync(request.Owner.OwnerFileIds, cancellationToken);
        if (filesOk.IsFailure) return filesOk.Error;

        property.Name = request.Name;
        property.Address = request.Address;
        property.CodeInternal = request.CodeInternal;
        property.Year = request.Year;
        property.CityId = request.CityId;

        var statusResult = property.SetStatus((PropertyStatusEnum)request.StatusId);
        if (statusResult.IsFailure) return statusResult.Error;

        if (request.Price.HasValue && request.PriceDate.HasValue)
        {
            var priceResult = property.ChangePrice(request.Price.Value, request.PriceDate.Value, request.Tax!.Value);
            if (priceResult.IsFailure) return priceResult.Error;
        }

        var ownerTargetResult = await EnsureOwnerAsync(property.Owner, request.Owner, cancellationToken);
        if (ownerTargetResult.IsFailure) return ownerTargetResult.Error;
        var ownerTarget = ownerTargetResult.Value;

        if (ownerTarget.Id != property.OwnerId)
        {
            property.OwnerId = ownerTarget.Id;
        }

        property.Images.Clear();
        foreach (var id in request.PropertyFileIds.Distinct())
        {
            if (id == Guid.Empty) continue;
            property.Images.Add(new Domain.Properties.PropertyImage
            {
                PropertyId = property.Id,
                FileId = id
            });
        }

        ownerTarget.OwnerImages.Clear();
        foreach (var id in request.Owner.OwnerFileIds.Distinct())
        {
            if (id == Guid.Empty) continue;
            ownerTarget.OwnerImages.Add(new Domain.Owners.OwnerImage
            {
                OwnerId = ownerTarget.Id,
                FileId = id
            });
        }

        await _uow.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result<Owner>> EnsureOwnerAsync(Owner currentOwner, OwnerUpdate input, CancellationToken ct)
    {
        var sameIdentification = currentOwner.IdentificationTypeId == input.IdentificationTypeId &&
                                  string.Equals(currentOwner.IdentificationNumber, input.IdentificationNumber, StringComparison.OrdinalIgnoreCase);

        if (sameIdentification)
        {
            currentOwner.Name = input.Name;
            currentOwner.Address = input.Address;
            currentOwner.BirthDate = input.BirthDate;
            return currentOwner;
        }

        var existing = await _owners.GetByIdentificationAsync(input.IdentificationTypeId, input.IdentificationNumber, ct);
        if (existing is not null)
        {
            existing.Name = input.Name;
            existing.Address = input.Address;
            existing.BirthDate = input.BirthDate;
            return existing;
        }

        var created = new Owner
        {
            Name = input.Name,
            Address = input.Address,
            BirthDate = input.BirthDate,
            IdentificationTypeId = input.IdentificationTypeId,
            IdentificationNumber = input.IdentificationNumber
        };

        _owners.Add(created);
        return created;
    }

    private async Task<Result> ValidateFilesExistAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var distinct = ids?.Where(id => id != Guid.Empty).Distinct().ToList() ?? new List<Guid>();
        if (distinct.Count == 0) return Result.Success();

        var existsCount = await _files.CountAsync(f => distinct.Contains(f.Id), ct);
        if (existsCount != distinct.Count)
            return Error.NotFound("One or more files were not found.");

        return Result.Success();
    }
}
