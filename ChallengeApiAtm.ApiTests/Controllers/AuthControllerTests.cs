
using System.Net;
using System.Text.Json;
using ChallengeApiAtm.ApiTests.Fixtures;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.ApiTests.Controllers;

/// <summary>
/// Tests for the AuthController class
/// </summary>
public class AuthControllerTests : BaseApiTest
{
    public AuthControllerTests(TestWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnSuccessWithToken()
    {
        var loginRequest = new
        {
            cardNumber = TestData.CardNumber,
            pin = TestData.Pin
        };
        var response = await PostJsonAsync("/api/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);

        jsonDoc.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
        jsonDoc.RootElement.GetProperty("data").GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        jsonDoc.RootElement.GetProperty("data").GetProperty("tokenType").GetString().Should().Be("Bearer");
        jsonDoc.RootElement.GetProperty("data").GetProperty("userInfo").GetProperty("fullName").GetString().Should().Be("Edwin Garcia");
    }

    [Fact]
    public async Task Login_InvalidPin_ShouldReturnUnauthorized()
    {
        var loginRequest = new
        {
            cardNumber = TestData.CardNumber,
            pin = "9999"
        };


        var response = await PostJsonAsync("/api/auth/login", loginRequest);


        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);

        jsonDoc.RootElement.GetProperty("success").GetBoolean().Should().BeFalse();
        jsonDoc.RootElement.GetProperty("error").GetString().Should().Be("PIN_INCORRECTO");
    }

    [Fact]
    public async Task Login_InvalidCardNumber_ShouldReturnUnauthorized()
    {
        
        var loginRequest = new
        {
            cardNumber = "9999999999999999",
            pin = TestData.Pin
        };

        
        var response = await PostJsonAsync("/api/auth/login", loginRequest);

        
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("", "8033")] 
    [InlineData("4001234567890101", "")] 
    [InlineData("123", "8033")] 
    [InlineData("4001234567890101", "12")] 
    public async Task Login_InvalidRequestData_ShouldReturnBadRequest(string cardNumber, string pin)
    {
        
        var loginRequest = new
        {
            cardNumber,
            pin
        };

        
        var response = await PostJsonAsync("/api/auth/login", loginRequest);

        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}