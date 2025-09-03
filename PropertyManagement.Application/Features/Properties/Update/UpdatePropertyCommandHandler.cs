using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Files;
using PropertyManagement.Domain.Owners;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Properties.Update;

public sealed class UpdatePropertyCommandHandler(
    IPropertyRepository propertyRepository,
    IFileRepository fileReposistory,
    IOwnerRepository ownerRepository,
    IUnitOfWork uow)
    : ICommandHandler<UpdatePropertyCommand>
{
    private readonly IPropertyRepository _propertyRepository = propertyRepository;
    private readonly IFileRepository _fileRepository = fileReposistory;
    private readonly IOwnerRepository _ownerRepository = ownerRepository;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetDetailsByIdAsync(request.Id, cancellationToken);

        if (property is null) return Error.NotFound("Property not found.");

        if (property.CodeInternal != request.CodeInternal && 
            await _propertyRepository.ExistsAsync(p => p.Id != property.Id && p.CodeInternal == request.CodeInternal, cancellationToken))
        {
            return Error.Conflict("A property with the same internal code already exists.");
        }

        var filesAreValid = await ValidateFilesExistAsync(
            request.PropertyFileIds.Concat(request.Owner.OwnerFileIds).Distinct(), cancellationToken);

        if (filesAreValid.IsFailure) return filesAreValid.Error;

        property.Name = request.Name;
        property.Address = request.Address;
        property.CodeInternal = request.CodeInternal;
        property.Year = request.Year;
        property.CityId = request.CityId;

        var statusResult = property.SetStatus((PropertyStatusEnum)request.StatusId);
        if (statusResult.IsFailure) return statusResult.Error;

        if (request.Price.HasValue)
        {
            var priceDate = DateTime.UtcNow;
            var tax = request.Tax ?? 0m;
            var priceResult = property.ChangePrice(request.Price.Value, priceDate, tax);
            if (priceResult.IsFailure) return priceResult.Error;
        }

        var ownerTarget = await EnsureOwnerAsync(property.Owner, request.Owner, cancellationToken);

        if (ownerTarget.Id != property.OwnerId)
        {
            property.OwnerId = ownerTarget.Id;
        }

        property.Images.Clear();
        foreach (var id in request.PropertyFileIds.Distinct())
        {
            property.Images.Add(new PropertyImage
            {
                PropertyId = property.Id,
                FileId = id
            });
        }

        ownerTarget.OwnerImages.Clear();
        foreach (var id in request.Owner.OwnerFileIds.Distinct())
        {
            ownerTarget.OwnerImages.Add(new OwnerImage
            {
                OwnerId = ownerTarget.Id,
                FileId = id
            });
        }

        await _uow.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Owner> EnsureOwnerAsync(Owner currentOwner, OwnerUpdate ownerRequest, CancellationToken ct)
    {
        var sameIdentification = currentOwner.IdentificationTypeId == ownerRequest.IdentificationTypeId &&
                                  string.Equals(currentOwner.IdentificationNumber, ownerRequest.IdentificationNumber, StringComparison.OrdinalIgnoreCase);

        if (sameIdentification)
        {
            currentOwner.Name = ownerRequest.Name;
            currentOwner.Address = ownerRequest.Address;
            currentOwner.BirthDate = ownerRequest.BirthDate;
            return currentOwner;
        }

        var existing = await _ownerRepository.GetByIdentificationAsync(ownerRequest.IdentificationTypeId, ownerRequest.IdentificationNumber, ct);

        if (existing is not null)
        {
            existing.Name = ownerRequest.Name;
            existing.Address = ownerRequest.Address;
            existing.BirthDate = ownerRequest.BirthDate;
            return existing;
        }

        var created = new Owner
        {
            Name = ownerRequest.Name,
            Address = ownerRequest.Address,
            BirthDate = ownerRequest.BirthDate,
            IdentificationTypeId = ownerRequest.IdentificationTypeId,
            IdentificationNumber = ownerRequest.IdentificationNumber
        };

        _ownerRepository.Add(created);
        return created;
    }

    private async Task<Result> ValidateFilesExistAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var fileIdsToValidate = ids?.Where(id => id != Guid.Empty).ToList() ?? new List<Guid>();
        if (fileIdsToValidate.Count == 0) return Result.Success();

        var existsCount = await _fileRepository.CountAsync(file => fileIdsToValidate.Contains(file.Id), ct);
        if (existsCount != fileIdsToValidate.Count)
            return Error.NotFound("One or more files were not found.");

        return Result.Success();
    }
}
