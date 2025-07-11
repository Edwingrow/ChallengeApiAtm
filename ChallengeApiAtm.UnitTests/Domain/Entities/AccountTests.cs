using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Domain.Entities;

/// <summary>
/// Tests for the Account class
/// </summary>
public class AccountTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateAccount()
    {
        
        var accountNumber = "12345678";
        var userId = Guid.NewGuid();
        var initialBalance = 1000m;

        var account = new Account(accountNumber, userId, initialBalance);

        account.AccountNumber.Should().Be(accountNumber);
        account.UserId.Should().Be(userId);
        account.Balance.Should().Be(initialBalance);
        account.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Withdraw_ValidAmount_ShouldReduceBalance()
    {
        
        var account = new Account("12345678", Guid.NewGuid(), 1000);
        const decimal withdrawAmount = 100;

        var newBalance = account.Withdraw(withdrawAmount);

        newBalance.Should().Be(900);
        account.Balance.Should().Be(900);
    }

    [Fact]
    public void Withdraw_InsufficientFunds_ShouldThrowInsufficientFundsException()
    {
        
        var account = new Account("12345678", Guid.NewGuid(), 50);
        const decimal withdrawAmount = 100;

        
        var action = () => account.Withdraw(withdrawAmount);
        action.Should().Throw<InsufficientFundsException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Withdraw_InvalidAmount_ShouldThrowArgumentException(decimal amount)
    {
        var account = new Account("12345678", Guid.NewGuid(), 1000);

        var action = () => account.Withdraw(amount);
        action.Should().Throw<ArgumentException>()
            .WithMessage("El monto debe ser mayor a cero*");
    }

    [Fact]
    public void Constructor_NegativeInitialBalance_ShouldThrowArgumentException()
    {
        
        var accountNumber = "12345678";
        var userId = Guid.NewGuid();
        var initialBalance = -100m;

        
        var action = () => new Account(accountNumber, userId, initialBalance);
        action.Should().Throw<ArgumentException>()
            .WithMessage("El saldo inicial no puede ser negativo*");
    }
}