using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using PropertyManagement.Application.Features.Properties.GetById;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Shared.Results;
using System.Threading;

namespace PropertyManagement.Tests.Application.Features.Properties;

[TestFixture]
public class GetPropertyByIdQueryHandlerTests
{
    private GetPropertyByIdQueryHandler _handler;
    private IPropertyRepository _repository;

    [SetUp]
    public void Setup()
    {
        _repository = Substitute.For<IPropertyRepository>();
        _handler = new GetPropertyByIdQueryHandler(_repository);
    }

    [Test]
    public async Task Handle_Should_Return_NotFound_When_Property_Does_Not_Exist()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery(propertyId);

        _repository.GetDetailsByIdAsync(propertyId, Arg.Any<CancellationToken>())
                   .Returns((Property?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Property not found.");
        result.Error.ErrorType.Should().Be(ErrorTypeEnum.NotFound);
    }

    [Test]
    public async Task Handle_Should_Return_PropertyResponse_When_Property_Exists()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var cityId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery(propertyId);

        var property = CreateTestProperty(propertyId, ownerId, cityId);

        _repository.GetDetailsByIdAsync(propertyId, Arg.Any<CancellationToken>())
                   .Returns(property);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeTrue();
        var response = result.Value;
        response.Id.Should().Be(propertyId);
        response.Name.Should().Be("Apartamento en Zona Rosa");
        response.Address.Should().Be("Carrera 15 #93-47, Bogotá");
        response.Price.Should().Be(380000000m);
        response.CodeInternal.Should().Be(45678);
        response.Year.Should().Be(2022);
        await _repository.Received(1).GetDetailsByIdAsync(propertyId, Arg.Any<CancellationToken>());
    }

    private static Property CreateTestProperty(Guid propertyId, Guid ownerId, Guid cityId)
    {
        var property = new Property
        {
            Id = propertyId,
            Name = "Apartamento en Zona Rosa",
            Address = "Carrera 15 #93-47, Bogotá",
            CodeInternal = 45678,
            Year = 2022,
            CityId = cityId,
            OwnerId = ownerId,
        };

        property.SetStatus(PropertyStatusEnum.Listed);

        property.SetInitialPrice(380000000m);

        property.Status = new PropertyStatus { Id = 2, Name = "En Venta" };
        property.City = new PropertyManagement.Domain.Countries.City { Id = cityId, Name = "Bogotá" };
        property.Owner = new PropertyManagement.Domain.Owners.Owner 
        { 
            Id = ownerId, 
            Name = "Ana Lucía Mendoza",
            IdentificationTypeId = 1,
            IdentificationNumber = "52987654"
        };
        property.Images = new List<PropertyImage>();

        return property;
    }
}