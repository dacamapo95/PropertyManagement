using PropertyManagement.Domain.Owners;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Infrastructure.Authentication;

namespace PropertyManagement.Tests.Helpers;

/// <summary>
/// Helper class to create test objects with realistic data
/// </summary>
public static class TestDataBuilder
{
    public static Property CreateProperty(
        string name = "Casa Colonial en La Candelaria",
        string address = "Carrera 7 #12-45, Bogotá",
        decimal price = 450000000m,
        int codeInternal = 98765,
        int year = 2023,
        int statusId = 1)
    {
        var property = new Property
        {
            Id = Guid.NewGuid(),
            Name = name,
            Address = address,
            CodeInternal = codeInternal,
            Year = year,
            CityId = Guid.NewGuid(),
            OwnerId = Guid.NewGuid()
        };

        property.SetStatus((PropertyStatusEnum)statusId);

        property.SetInitialPrice(price);
        return property;
    }

    public static Owner CreateOwner(
        string name = "María Fernanda Jiménez",
        string identificationNumber = "52789456",
        int identificationTypeId = 1,
        string? address = "Avenida El Dorado #68-23, Bogotá")
    {
        return new Owner
        {
            Id = Guid.NewGuid(),
            Name = name,
            IdentificationNumber = identificationNumber,
            IdentificationTypeId = identificationTypeId,
            Address = address,
            BirthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-35))
        };
    }

    public static User CreateUser(
        string email = "carlos.rodriguez@inmobiliaria.com",
        string? userName = null)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = userName ?? email,
            EmailConfirmed = true,
            Tokens = new List<UserToken>()
        };
    }

    public static PropertyManagement.Domain.Files.File CreateFile(
        string name = "foto-fachada-casa",
        string originalName = "casa_colonial_fachada.jpg",
        string mimeType = "image/jpeg",
        string extension = ".jpg",
        long size = 2048)
    {
        return new PropertyManagement.Domain.Files.File
        {
            Id = Guid.NewGuid(),
            Name = name,
            OriginalName = originalName,
            MimeType = mimeType,
            Extension = extension,
            Size = size,
            Content = new byte[] { 255, 216, 255, 224, 0, 16, 74, 70 } // JPEG header bytes
        };
    }
}