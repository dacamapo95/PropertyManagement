using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Application.Features.Properties.Create;
using PropertyManagement.Domain.Files;
using PropertyManagement.Domain.Owners;
using PropertyManagement.Domain.Properties;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Tests.Application.Features.Properties;

[TestFixture]
public class CreatePropertyCommandHandlerTests
{
    private CreatePropertyCommandHandler _handler;
    private IPropertyRepository _propertyRepository;
    private IFileRepository _fileRepository;
    private IOwnerRepository _ownerRepository;
    private IUnitOfWork _unitOfWork;

    [SetUp]
    public void Setup()
    {
        _propertyRepository = Substitute.For<IPropertyRepository>();
        _fileRepository = Substitute.For<IFileRepository>();
        _ownerRepository = Substitute.For<IOwnerRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _handler = new CreatePropertyCommandHandler(
            _propertyRepository,
            _fileRepository,
            _ownerRepository,
            _unitOfWork);
    }

    [Test]
    public async Task Handle_Should_Return_Conflict_When_Property_With_Same_CodeInternal_Exists()
    {
        // Arrange
        var command = CreateTestCommand();
        var existingProperty = new Property { Id = Guid.NewGuid() };

        _propertyRepository.GetByCodeInternalAsync(command.CodeInternal, Arg.Any<CancellationToken>())
                          .Returns(existingProperty);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("already exists");
        result.Error.ErrorType.Should().Be(ErrorTypeEnum.Conflict);
    }

    [Test]
    public async Task Handle_Should_Return_NotFound_When_Files_Do_Not_Exist()
    {
        // Arrange
        var command = CreateTestCommand();
        var fileIds = new List<Guid> { Guid.NewGuid() };
        command = command with { PropertyFileIds = fileIds };

        _propertyRepository.GetByCodeInternalAsync(command.CodeInternal, Arg.Any<CancellationToken>())
                          .Returns((Property?)null);

        _ownerRepository.GetByIdentificationAsync(
            command.Owner.IdentificationTypeId,
            command.Owner.IdentificationNumber,
            Arg.Any<CancellationToken>())
            .Returns((Owner?)null);

        _fileRepository.CountAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Domain.Files.File, bool>>>(), Arg.Any<CancellationToken>())
                      .Returns(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("One or more files were not found.");
        result.Error.ErrorType.Should().Be(ErrorTypeEnum.NotFound);
    }

    [Test]
    public async Task Handle_Should_Return_ValidationError_When_StatusId_Is_Sold()
    {
        // Arrange
        var command = CreateTestCommand() with { StatusId = (int)PropertyStatusEnum.Sold };

        _propertyRepository.GetByCodeInternalAsync(command.CodeInternal, Arg.Any<CancellationToken>())
                          .Returns((Property?)null);

        _ownerRepository.GetByIdentificationAsync(
            command.Owner.IdentificationTypeId,
            command.Owner.IdentificationNumber,
            Arg.Any<CancellationToken>())
            .Returns((Owner?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Cannot create a property with Sold status.");
        result.Error.ErrorType.Should().Be(ErrorTypeEnum.Validation);
    }

    [Test]
    public async Task Handle_Should_Create_New_Owner_When_Owner_Does_Not_Exist()
    {
        // Arrange
        var command = CreateTestCommand();

        _propertyRepository.GetByCodeInternalAsync(command.CodeInternal, Arg.Any<CancellationToken>())
                          .Returns((Property?)null);

        _ownerRepository.GetByIdentificationAsync(
            command.Owner.IdentificationTypeId,
            command.Owner.IdentificationNumber,
            Arg.Any<CancellationToken>())
            .Returns((Owner?)null);

        _fileRepository.CountAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Domain.Files.File, bool>>>(), Arg.Any<CancellationToken>())
                      .Returns(0); // No files to validate

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeTrue();
        _ownerRepository.Received(1).Add(Arg.Any<Owner>());
        _propertyRepository.Received(1).Add(Arg.Any<Property>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_Should_Update_Existing_Owner_When_Owner_Exists()
    {
        // Arrange
        var command = CreateTestCommand();
        var existingOwner = new Owner
        {
            Id = Guid.NewGuid(),
            Name = "Pedro Jiménez",
            Address = "Dirección Antigua",
            IdentificationTypeId = command.Owner.IdentificationTypeId,
            IdentificationNumber = command.Owner.IdentificationNumber
        };

        _propertyRepository.GetByCodeInternalAsync(command.CodeInternal, Arg.Any<CancellationToken>())
                          .Returns((Property?)null);

        _ownerRepository.GetByIdentificationAsync(
            command.Owner.IdentificationTypeId,
            command.Owner.IdentificationNumber,
            Arg.Any<CancellationToken>())
            .Returns(existingOwner);

        _fileRepository.CountAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Domain.Files.File, bool>>>(), Arg.Any<CancellationToken>())
                      .Returns(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeTrue();
        existingOwner.Name.Should().Be(command.Owner.Name);
        existingOwner.Address.Should().Be(command.Owner.Address);
        _ownerRepository.DidNotReceive().Add(Arg.Any<Owner>());
        _propertyRepository.Received(1).Add(Arg.Any<Property>());
    }

    private static CreatePropertyCommand CreateTestCommand()
    {
        var ownerCommand = new OwnerCommand(
            1, 
            "52987456",
            "Luis Fernando Restrepo", 
            "Calle 72 #10-34, Medellín", 
            DateOnly.FromDateTime(DateTime.Today.AddYears(-42)),
            new List<Guid>() 
        );

        return new CreatePropertyCommand(
            "Casa Campestre en El Poblado", 
            "Carrera 43A #12-56, Medellín", 
            750000000m, 
            12, 
            2021, 
            (int)PropertyStatusEnum.Listed, 
            Guid.NewGuid(), 
            ownerCommand, 
            new List<Guid>() 
        );
    }
}