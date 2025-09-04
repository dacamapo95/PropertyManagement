using FluentAssertions;
using NSubstitute;
using PropertyManagement.Application.Core.Abstractions;
using PropertyManagement.Application.Features.Files.Delete;
using PropertyManagement.Domain.Files;
using PropertyManagement.Shared.Results;
using File = PropertyManagement.Domain.Files.File;

namespace PropertyManagement.Tests.Application.Features.Files;

[TestFixture]
public class DeleteFileCommandHandlerTests
{
    private DeleteFileCommandHandler _handler;
    private IFileRepository _fileRepository;
    private IUnitOfWork _unitOfWork;

    [SetUp]
    public void Setup()
    {
        _fileRepository = Substitute.For<IFileRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new DeleteFileCommandHandler(_fileRepository, _unitOfWork);
    }

    [Test]
    public async Task Handle_Should_Return_NotFound_When_File_Does_Not_Exist()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var command = new DeleteFileCommand(fileId);

        _fileRepository.GetByIdAsync(fileId, Arg.Any<CancellationToken>())
                      .Returns((Domain.Files.File?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.ErrorType.Should().Be(ErrorTypeEnum.NotFound);
        result.Error.Message.Should().Be("File not found.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_Should_Delete_File_When_File_Exists()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var command = new DeleteFileCommand(fileId);
        var file = new File
        {
            Id = fileId,
            Name = "foto-sala-principal",
            OriginalName = "apartamento_sala_vista.jpg",
            MimeType = "image/jpeg",
            Extension = ".jpg",
            Size = 3072
        };

        _fileRepository.GetByIdAsync(fileId, Arg.Any<CancellationToken>())
                      .Returns(file);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeTrue();
        _fileRepository.Received(1).Remove(file);
        await _fileRepository.Received(1).GetByIdAsync(fileId, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}