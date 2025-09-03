using PropertyManagement.Domain.Owners;
using PropertyManagement.Domain.Properties;

namespace PropertyManagement.Application.Features.Properties.GetById;

public static class PropertyMappingExtensions
{
    public static OwnerResponse ToOwnerResponse(this Owner owner)
    {
        var images = owner.OwnerImages
            .Select(oi => new OwnerImageResponse(
                oi.FileId,
                oi.File.OriginalName,
                oi.File.Name,
                oi.File.MimeType,
                oi.File.Size,
                oi.File.Content is null ? string.Empty : Convert.ToBase64String(oi.File.Content)))
            .ToList();

        return new OwnerResponse(
            owner.Id,
            owner.Name,
            owner.Address,
            owner.BirthDate,
            owner.IdentificationTypeId,
            owner.IdentificationNumber,
            images);
    }

    public static List<PropertyImageResponse> ToPropertyImageResponses(this IEnumerable<PropertyImage> images)
    {
        return images
            .Select(pi => new PropertyImageResponse(
                pi.FileId,
                pi.File.Name,   
                pi.File.OriginalName,
                pi.File.MimeType,
                pi.File.Size,
                pi.File.Content is null ? string.Empty : Convert.ToBase64String(pi.File.Content)))
            .ToList();
    }

    public static PropertyResponse ToPropertyResponse(this Property property)
    {
        var owner = property.Owner.ToOwnerResponse();
        var images = property.Images.ToPropertyImageResponses();

        return new PropertyResponse(
            property.Id,
            property.Name,
            property.Address,
            property.Price,
            property.CodeInternal,
            property.Year,
            property.StatusId,
            property.Status.Name,
            property.CityId,
            property.City.Name,
            owner,
            images
        );
    }
}
