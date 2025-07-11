using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Domain.Entities;

/// <summary>
/// Tests for the Transaction class
/// </summary>
public class TransactionTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateTransaction()
    {

        var accountId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var type = TransactionType.Withdrawal;
        var amount = 100.50m;
        var description = "Retiro ATM";


        var transaction = new Transaction(accountId, cardId, type, amount, description);


        transaction.AccountId.Should().Be(accountId);
        transaction.CardId.Should().Be(cardId);
        transaction.Type.Should().Be(type);
        transaction.Amount.Should().Be(amount);
        transaction.Description.Should().Be(description);
        transaction.Status.Should().Be(TransactionStatus.Completed);
        transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithBalanceAfterTransaction_ShouldSetBalance()
    {

        var accountId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var balanceAfter = 850.75m;


        var transaction = new Transaction(accountId, cardId, TransactionType.Withdrawal, 100, "Test", balanceAfter);


        transaction.BalanceAfterTransaction.Should().Be(balanceAfter);
    }

    [Fact]
    public void Constructor_WithdrawalWithNegativeAmount_ShouldThrowArgumentException()
    {

        var accountId = Guid.NewGuid();
        var cardId = Guid.NewGuid();


        var action = () => new Transaction(accountId, cardId, TransactionType.Withdrawal, -100, "Test");
        action.Should().Throw<ArgumentException>()
            .WithMessage("Los retiros deben tener monto positivo*");
    }

    [Fact]
    public void Constructor_WithdrawalWithZeroAmount_ShouldThrowArgumentException()
    {

        var accountId = Guid.NewGuid();
        var cardId = Guid.NewGuid();


        var action = () => new Transaction(accountId, cardId, TransactionType.Withdrawal, 0, "Test");
        action.Should().Throw<ArgumentException>()
            .WithMessage("Los retiros deben tener monto positivo*");
    }

    [Fact]
    public void Constructor_EmptyDescription_ShouldThrowArgumentException()
    {

        var accountId = Guid.NewGuid();
        var cardId = Guid.NewGuid();


        var action = () => new Transaction(accountId, cardId, TransactionType.Withdrawal, 100, "");
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_EmptyAccountId_ShouldThrowArgumentException()
    {

        var action = () => new Transaction(Guid.Empty, Guid.NewGuid(), TransactionType.Withdrawal, 100, "Test");
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_EmptyCardId_ShouldThrowArgumentException()
    {

        var action = () => new Transaction(Guid.NewGuid(), Guid.Empty, TransactionType.Withdrawal, 100, "Test");
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_BalanceInquiryWithZeroAmount_ShouldBeValid()
    {

        var transaction = new Transaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TransactionType.BalanceInquiry,
            0,
            "Consulta de saldo");


        transaction.Amount.Should().Be(0);
        transaction.Type.Should().Be(TransactionType.BalanceInquiry);
    }

    [Fact]
    public void Constructor_BalanceInquiryWithNegativeAmount_ShouldBeValid()
    {

        var transaction = new Transaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TransactionType.BalanceInquiry,
            -50,
            "Ajuste de saldo");


        transaction.Amount.Should().Be(-50);
        transaction.Type.Should().Be(TransactionType.BalanceInquiry);
    }

    [Fact]
    public void Constructor_WithBalanceAfterTransaction_WithdrawalWithNegativeAmount_ShouldThrowArgumentException()
    {

        var accountId = Guid.NewGuid();
        var cardId = Guid.NewGuid();


        var action = () => new Transaction(accountId, cardId, TransactionType.Withdrawal, -100, "Test", 500m);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Los retiros deben tener monto positivo*");
    }

    [Fact]
    public void Constructor_WithBalanceAfterTransaction_WithdrawalWithZeroAmount_ShouldThrowArgumentException()
    {

        var accountId = Guid.NewGuid();
        var cardId = Guid.NewGuid();


        var action = () => new Transaction(accountId, cardId, TransactionType.Withdrawal, 0, "Test", 500m);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Los retiros deben tener monto positivo*");
    }
}