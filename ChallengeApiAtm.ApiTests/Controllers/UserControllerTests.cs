
using System.Net;
using System.Text.Json;
using ChallengeApiAtm.ApiTests.Fixtures;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.ApiTests.Controllers;

/// <summary>
/// Tests for the UserController class
/// </summary>
public class UserControllerTests : BaseApiTest
{
    public UserControllerTests(TestWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task Register_ValidRequest_ShouldReturnCreated()
    {
        
        var registerRequest = new
        {
            firstName = "Juan",
            lastName = "Pérez",
            documentNumber = "87654321",
            cardNumber = "4001234567890999",
            expiryDate = "12/2026",
            pin = "1234",
            confirmPin = "1234",
            initialBalance = 1500000m
        };

        
        var response = await PostJsonAsync("/api/user/register", registerRequest);

        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        
        jsonDoc.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
        jsonDoc.RootElement.GetProperty("data").GetProperty("firstName").GetString().Should().Be("Juan");
        jsonDoc.RootElement.GetProperty("data").GetProperty("lastName").GetString().Should().Be("Pérez");
        jsonDoc.RootElement.GetProperty("data").GetProperty("initialBalance").GetDecimal().Should().Be(1500000m);
    }

    [Fact]
    public async Task Register_DuplicateDocument_ShouldReturnConflict()
    {
        
        var registerRequest = new
        {
            firstName = "Otro",
            lastName = "Usuario",
            documentNumber = TestData.DocumentNumber, 
            cardNumber = "4001234567890888",
            expiryDate = "12/2026",
            pin = "1234",
            confirmPin = "1234",
            initialBalance = 1500000m
        };

        
        var response = await PostJsonAsync("/api/user/register", registerRequest);

        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("", "Pérez", "12345678", "El nombre es obligatorio")]
    [InlineData("Juan", "", "12345678", "El apellido es obligatorio")]
    [InlineData("Juan", "Pérez", "", "El número de documento es obligatorio")]
    [InlineData("Juan", "Pérez", "12345678", "")] 
    public async Task Register_InvalidData_ShouldReturnBadRequest(
        string firstName, string lastName, string documentNumber, string pin)
    {
        
        var registerRequest = new
        {
            firstName,
            lastName,
            documentNumber,
            cardNumber = "4001234567890777",
            expiryDate = "12/2026",
            pin,
            confirmPin = pin,
            initialBalance = 1500000m
        };

        
        var response = await PostJsonAsync("/api/user/register", registerRequest);

        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}