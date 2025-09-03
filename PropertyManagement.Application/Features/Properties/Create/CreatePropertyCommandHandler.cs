using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Domain.Owners;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Application.Features.Properties.Create;

public sealed class CreatePropertyCommandHandler(
    IPropertyRepository properties,
    Domain.Files.IFileRepository files,
    IOwnerRepository owners,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreatePropertyCommand, CreatePropertyResponse>
{
    private readonly IPropertyRepository _properties = properties;
    private readonly Domain.Files.IFileRepository _files = files;
    private readonly IOwnerRepository _ownerRepository = owners;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CreatePropertyResponse>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var ownerResult = await GetOrCreateOwnerAsync(request.Owner, cancellationToken);
        if (ownerResult.IsFailure) return ownerResult.Error;
        var owner = ownerResult.Value;

        var filesOk = await ValidateFilesExistAsync(request.PropertyFileIds, cancellationToken);
        if (filesOk.IsFailure) return filesOk.Error;

        filesOk = await ValidateFilesExistAsync(request.Owner.OwnerFileIds, cancellationToken);
        if (filesOk.IsFailure) return filesOk.Error;

        var property = BuildProperty(request, owner);

        var statusResult = ApplyStatus(property, request.StatusId);
        if (statusResult.IsFailure) return statusResult.Error;

        var priceResult = ApplyInitialPrice(property, request.Price);
        if (priceResult.IsFailure) return priceResult.Error;

        AttachPropertyImages(property, request.PropertyFileIds);
        AttachOwnerImages(owner, request.Owner.OwnerFileIds);

        _properties.Add(property); 

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreatePropertyResponse(property.Id);
    }

    private async Task<Result<Owner>> GetOrCreateOwnerAsync(OwnerCommand input, CancellationToken ct)
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
        var distinct = ids?.Where(id => id != Guid.Empty).Distinct().ToList() ?? new List<Guid>();
        if (distinct.Count == 0) return Result.Success();

        var existsCount = await _files.CountAsync(f => distinct.Contains(f.Id), ct);
        if (existsCount != distinct.Count)
            return Error.NotFound("One or more files were not found.");

        return Result.Success();
    }

    private static Property BuildProperty(CreatePropertyCommand request, Owner owner)
    {
        return new Property
        {
            Name = request.Name,
            Address = request.Address,
            CodeInternal = request.CodeInternal,
            Year = request.Year,
            CountryId = request.CountryId,
            StateId = request.StateId,
            CityId = request.CityId,
            OwnerId = owner.Id,
        };
    }

    private static Result ApplyStatus(Property property, int statusId)
    {
        var result = property.SetStatus((PropertyStatusEnum)statusId);
        return result.IsFailure ? result.Error : Result.Success();
    }

    private static Result ApplyInitialPrice(Property property, decimal price)
    {
        var result = property.ChangePrice(price, DateOnly.FromDateTime(DateTime.UtcNow));
        return result.IsFailure ? result.Error : Result.Success();
    }

    private static void AttachPropertyImages(Property property, IEnumerable<Guid> fileIds)
    {
        foreach (var id in fileIds.Distinct())
        {
            if (id == Guid.Empty) continue;
            property.Images.Add(new PropertyImage
            {
                PropertyId = property.Id,
                FileId = id
            });
        }
    }

    private static void AttachOwnerImages(Owner owner, IEnumerable<Guid> fileIds)
    {
        foreach (var id in fileIds.Distinct())
        {
            if (id == Guid.Empty) continue;
            owner.OwnerImages.Add(new OwnerImage
            {
                OwnerId = owner.Id,
                FileId = id
            });
        }
    }
}
