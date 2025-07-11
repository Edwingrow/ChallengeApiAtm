
using System.Net;
using System.Text.Json;
using ChallengeApiAtm.ApiTests.Fixtures;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.ApiTests.Controllers;

/// <summary>
/// Tests for the AccountController class
/// </summary>
public class AccountControllerTests : BaseApiTest
{
    public AccountControllerTests(TestWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task GetBalance_WithValidToken_ShouldReturnBalance()
    {
        var token = await LoginAndGetTokenAsync();
        SetAuthorization(token);

        var balanceRequest = new
        {
            cardNumber = TestData.CardNumber
        };

        var response = await PostJsonAsync("/api/account/balance", balanceRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);

        jsonDoc.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
        jsonDoc.RootElement.GetProperty("data").GetProperty("userName").GetString().Should().Be("Edwin Garcia");
        jsonDoc.RootElement.GetProperty("data").GetProperty("currentBalance").GetDecimal().Should().Be(TestData.InitialBalance);
    }

    [Fact]
    public async Task GetBalance_WithoutToken_ShouldReturnUnauthorized()
    {
        var balanceRequest = new
        {
            cardNumber = TestData.CardNumber
        };

        var response = await PostJsonAsync("/api/account/balance", balanceRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Withdraw_ValidAmount_ShouldReturnSuccess()
    {
        var token = await LoginAndGetTokenAsync();
        SetAuthorization(token);

        var withdrawRequest = new
        {
            cardNumber = TestData.CardNumber,
            amount = 50000m
        };

        var response = await PostJsonAsync("/api/account/withdraw", withdrawRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);

        jsonDoc.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
        jsonDoc.RootElement.GetProperty("data").GetProperty("withdrawnAmount").GetDecimal().Should().Be(50000m);
        jsonDoc.RootElement.GetProperty("data").GetProperty("newBalance").GetDecimal().Should().Be(TestData.InitialBalance - 50000m);
    }

    [Fact]
    public async Task Withdraw_InsufficientFunds_ShouldReturnBadRequest()
    {
        var token = await LoginAndGetTokenAsync();
        SetAuthorization(token);

        var withdrawRequest = new
        {
            cardNumber = TestData.CardNumber,
            amount = 2000000m
        };

        var response = await PostJsonAsync("/api/account/withdraw", withdrawRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);

        jsonDoc.RootElement.GetProperty("success").GetBoolean().Should().BeFalse();
        jsonDoc.RootElement.GetProperty("error").GetString().Should().Be("FONDOS_INSUFICIENTES");
    }

    [Fact]
    public async Task GetOperations_ValidRequest_ShouldReturnOperations()
    {
        var token = await LoginAndGetTokenAsync();
        SetAuthorization(token);

        await PostJsonAsync("/api/account/withdraw", new
        {
            cardNumber = TestData.CardNumber,
            amount = 100000m
        });

        var operationsRequest = new
        {
            cardNumber = TestData.CardNumber
        };


        var response = await PostJsonAsync("/api/account/operations?pageNumber=1&pageSize=10", operationsRequest);


        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);

        jsonDoc.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
        jsonDoc.RootElement.GetProperty("data").GetProperty("transactions").GetArrayLength().Should().BeGreaterThan(0);
        jsonDoc.RootElement.GetProperty("data").GetProperty("pagination").GetProperty("currentPage").GetInt32().Should().Be(1);
    }
}