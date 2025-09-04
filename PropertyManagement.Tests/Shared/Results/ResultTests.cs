using FluentAssertions;
using NUnit.Framework;
using PropertyManagement.Shared.Results;

namespace PropertyManagement.Tests.Shared.Results;

[TestFixture]
public class ResultTests
{
    [Test]
    public void Success_Should_Create_Valid_Result()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsValid.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Test]
    public void Success_Generic_Should_Create_Valid_Result_With_Value()
    {
        // Arrange
        var expectedValue = "Casa en Zona Rosa";

        // Act
        var result = Result.Success(expectedValue);

        // Assert
        result.IsValid.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(expectedValue);
        result.Error.Should().Be(Error.None);
    }

    [Test]
    public void Fail_Should_Create_Invalid_Result_With_Error()
    {
        // Arrange
        var error = Error.NotFound("Propiedad no encontrada");

        // Act
        var result = Result.Fail(error);

        // Assert
        result.IsValid.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Test]
    public void Fail_Generic_Should_Create_Invalid_Result_With_Error()
    {
        // Arrange
        var error = Error.Validation("Precio debe ser mayor a cero");

        // Act
        var result = Result.Fail<decimal>(error);

        // Assert
        result.IsValid.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Test]
    public void ImplicitOperator_Should_Convert_Error_To_Failed_Result()
    {
        // Arrange
        var error = Error.Conflict("Propiedad ya existe");

        // Act
        Result result = error;

        // Assert
        result.IsValid.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Test]
    public void Success_Should_Be_Immutable()
    {
        // Act
        var result1 = Result.Success();
        var result2 = Result.Success();

        // Assert
        result1.IsValid.Should().Be(result2.IsValid);
        result1.Error.Should().Be(result2.Error);
    }

    [Test]
    public void Fail_Should_Preserve_Error_Details()
    {
        // Arrange
        var errorMessage = "El código interno de la propiedad ya está en uso";
        var error = Error.Conflict(errorMessage);

        // Act
        var result = Result.Fail(error);

        // Assert
        result.Error.Message.Should().Be(errorMessage);
        result.Error.ErrorType.Should().Be(ErrorTypeEnum.Conflict);
    }
}