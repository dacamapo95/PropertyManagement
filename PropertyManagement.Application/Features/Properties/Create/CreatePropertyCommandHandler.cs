using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Files;
using PropertyManagement.Domain.Owners;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Properties.Create;

public sealed class CreatePropertyCommandHandler(
    IPropertyRepository propertyRepositoy,
    IFileRepository fileRepository,
    IOwnerRepository ownerRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreatePropertyCommand, CreatePropertyResponse>
{
    private readonly IPropertyRepository _propertyRepository = propertyRepositoy;
    private readonly IFileRepository _fileRepository = fileRepository;
    private readonly IOwnerRepository _ownerRepository = ownerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CreatePropertyResponse>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var existing = await _propertyRepository.GetByCodeInternalAsync(request.CodeInternal, cancellationToken);

        if (existing is not null)
        {
            return Error.Conflict($"A property with code {request.CodeInternal} already exists.");
        }

        var owner = await GetOrCreateOwnerAsync(request.Owner, cancellationToken);

        var fileIdsAreValidResult = await ValidateFilesExistAsync(
            request.PropertyFileIds.Concat(request.Owner.OwnerFileIds).Where(id => id != Guid.Empty).Distinct(), 
            cancellationToken);

        if (fileIdsAreValidResult.IsFailure) return fileIdsAreValidResult.Error;

        var property = BuildProperty(request, owner);

        var statusResult = ApplyStatus(property, request.StatusId);
        if (statusResult.IsFailure) return statusResult.Error;

        var priceResult = ApplyInitialPrice(property, request.Price);
        if (priceResult.IsFailure) return priceResult.Error;

        AttachPropertyImages(property, request.PropertyFileIds);
        AttachOwnerImages(owner, request.Owner.OwnerFileIds);

        _propertyRepository.Add(property); 

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreatePropertyResponse(property.Id);
    }

    private async Task<Owner> GetOrCreateOwnerAsync(OwnerCommand input, CancellationToken ct)
    {
        var owner = await _ownerRepository.GetByIdentificationAsync(input.IdentificationTypeId, input.IdentificationNumber, ct);
        if (owner is not null)
        {
            owner.Name = input.Name;
            owner.Address = input.Address;
            owner.BirthDate = input.BirthDate;
            return owner;
        }

        owner = new Owner
        {
            Name = input.Name,
            Address = input.Address,
            BirthDate = input.BirthDate,
            IdentificationTypeId = input.IdentificationTypeId,
            IdentificationNumber = input.IdentificationNumber
        };

        _ownerRepository.Add(owner);

        return owner;
    }

    private async Task<Result> ValidateFilesExistAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        if (!ids.Any())
            return Result.Success();

        var distinctFileIds = ids?.Where(id => id != Guid.Empty).Distinct().ToList() ?? new List<Guid>();

        var existsCount = await _fileRepository.CountAsync(f => distinctFileIds.Contains(f.Id), ct);
        if (existsCount != distinctFileIds.Count)
            return Error.NotFound("One or more files were not found.");

        return Result.Success();
    }

    private Property BuildProperty(CreatePropertyCommand request, Owner owner)
    {
        return new Property
        {
            Name = request.Name,
            Address = request.Address,
            CodeInternal = request.CodeInternal,
            Year = request.Year,
            CityId = request.CityId,
            OwnerId = owner.Id,
        };
    }

    private Result ApplyStatus(Property property, int statusId)
    {
        if ((PropertyStatusEnum)statusId == PropertyStatusEnum.Sold)
            return Error.Validation("Cannot create a property with Sold status.");

        var result = property.SetStatus((PropertyStatusEnum)statusId);
        return result.IsFailure ? result.Error : Result.Success();
    }

    private Result ApplyInitialPrice(Property property, decimal price)
    {
        return property.SetInitialPrice(price);
    }

    private void AttachPropertyImages(Property property, IEnumerable<Guid> fileIds)
    {
        foreach (var id in fileIds.Distinct())
        {
            property.Images.Add(new PropertyImage
            {
                PropertyId = property.Id,
                FileId = id
            });
        }
    }

    private void AttachOwnerImages(Owner owner, IEnumerable<Guid> fileIds)
    {
        foreach (var id in fileIds.Distinct())
        {
            owner.OwnerImages.Add(new OwnerImage
            {
                OwnerId = owner.Id,
                FileId = id
            });
        }
    }
}
