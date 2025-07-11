using ChallengeApiAtm.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Domain.Entities;

/// <summary>
/// Tests for the User class
/// </summary>
public class UserTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateUser()
    {
        var firstName = "Juan";
        var lastName = "Pérez";
        var documentNumber = "12345678";

        var user = new User(firstName, lastName, documentNumber);

        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.DocumentNumber.Should().Be(documentNumber);
        user.FullName.Should().Be("Juan Pérez");
        user.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Pérez", "12345678", "El nombre es requerido")]
    [InlineData(null, "Pérez", "12345678", "El nombre es requerido")]
    [InlineData("Juan", "", "12345678", "El apellido es requerido")]
    [InlineData("Juan", null, "12345678", "El apellido es requerido")]
    [InlineData("Juan", "Pérez", "", "El número de documento es requerido")]
    [InlineData("Juan", "Pérez", null, "El número de documento es requerido")]
    public void Constructor_InvalidParameters_ShouldThrowArgumentException(
        string? firstName, string? lastName, string? documentNumber, string expectedMessage)
    {

        var action = () => new User(firstName!, lastName!, documentNumber!);
        action.Should().Throw<ArgumentException>()
            .WithMessage($"{expectedMessage}*");
    }

    [Fact]
    public void FullName_ShouldReturnCombinedFirstAndLastName()
    {
        var user = new User("María José", "García López", "87654321");
        user.FullName.Should().Be("María José García López");
    }

    [Fact]
    public void Constructor_ShouldTrimWhitespace()
    {
        var user = new User("  Juan  ", "  Pérez  ", "  12345678  ");
        user.FirstName.Should().Be("Juan");
        user.LastName.Should().Be("Pérez");
        user.DocumentNumber.Should().Be("12345678");
    }
}