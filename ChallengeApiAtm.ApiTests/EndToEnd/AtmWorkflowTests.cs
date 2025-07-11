
using System.Net;
using System.Text.Json;
using ChallengeApiAtm.ApiTests.Fixtures;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.ApiTests.EndToEnd;

/// <summary>
/// Tests de flujo completo End-to-End que simulan el uso real del ATM
/// </summary>
public class AtmWorkflowTests : BaseApiTest
{
    public AtmWorkflowTests(TestWebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task CompleteAtmWorkflow_ShouldWorkEndToEnd()
    {
        
        var token = await LoginAndGetTokenAsync();
        SetAuthorization(token);

        
        var balanceResponse = await PostJsonAsync("/api/account/balance", new
        {
            cardNumber = TestData.CardNumber
        });
        
        balanceResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var balanceContent = await balanceResponse.Content.ReadAsStringAsync();
        var balanceJson = JsonDocument.Parse(balanceContent);
        var initialBalance = balanceJson.RootElement.GetProperty("data").GetProperty("currentBalance").GetDecimal();
        initialBalance.Should().Be(TestData.InitialBalance);

        
        var withdrawAmount = 150000m;
        var withdrawResponse = await PostJsonAsync("/api/account/withdraw", new
        {
            cardNumber = TestData.CardNumber,
            amount = withdrawAmount
        });

        withdrawResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var withdrawContent = await withdrawResponse.Content.ReadAsStringAsync();
        var withdrawJson = JsonDocument.Parse(withdrawContent);
        var newBalance = withdrawJson.RootElement.GetProperty("data").GetProperty("newBalance").GetDecimal();
        newBalance.Should().Be(initialBalance - withdrawAmount);

        
        var balanceAfterWithdrawResponse = await PostJsonAsync("/api/account/balance", new
        {
            cardNumber = TestData.CardNumber
        });

        balanceAfterWithdrawResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var balanceAfterContent = await balanceAfterWithdrawResponse.Content.ReadAsStringAsync();
        var balanceAfterJson = JsonDocument.Parse(balanceAfterContent);
        var currentBalance = balanceAfterJson.RootElement.GetProperty("data").GetProperty("currentBalance").GetDecimal();
        currentBalance.Should().Be(newBalance);

        
        var operationsResponse = await PostJsonAsync("/api/account/operations?pageNumber=1&pageSize=10", new
        {
            cardNumber = TestData.CardNumber
        });

        operationsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var operationsContent = await operationsResponse.Content.ReadAsStringAsync();
        var operationsJson = JsonDocument.Parse(operationsContent);
        var transactions = operationsJson.RootElement.GetProperty("data").GetProperty("transactions");
        transactions.GetArrayLength().Should().BeGreaterThan(0);

        
        var withdrawTransaction = transactions.EnumerateArray()
            .FirstOrDefault(t => t.GetProperty("type").GetInt32() == 1); 
        
        withdrawTransaction.ValueKind.Should().NotBe(JsonValueKind.Undefined);
        withdrawTransaction.GetProperty("amount").GetDecimal().Should().Be(withdrawAmount);
    }

    [Fact]
    public async Task RegisterNewUser_ThenUseAtm_ShouldWork()
    {
        
        var registerRequest = new
        {
            firstName = "Maria",
            lastName = "Lopez",
            documentNumber = "99887766",
            cardNumber = "4001234567890555",
            expiryDate = "12/2026",
            pin = "9999",
            confirmPin = "9999",
            initialBalance = 2000000m
        };

        var registerResponse = await PostJsonAsync("/api/user/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        
        var loginRequest = new
        {
            cardNumber = registerRequest.cardNumber,
            pin = registerRequest.pin
        };

        var loginResponse = await PostJsonAsync("/api/auth/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent);
        var token = loginJson.RootElement.GetProperty("data").GetProperty("token").GetString();
        
        SetAuthorization(token!);

        
        var balanceResponse = await PostJsonAsync("/api/account/balance", new
        {
            cardNumber = registerRequest.cardNumber
        });

        balanceResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var balanceContent = await balanceResponse.Content.ReadAsStringAsync();
        var balanceJson = JsonDocument.Parse(balanceContent);
        var balance = balanceJson.RootElement.GetProperty("data").GetProperty("currentBalance").GetDecimal();
        balance.Should().Be(registerRequest.initialBalance);
    }
}