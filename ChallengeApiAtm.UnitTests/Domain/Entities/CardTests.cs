using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Enums;
using ChallengeApiAtm.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Domain.Entities;

/// <summary>
/// Tests for the Card class
/// </summary>
public class CardTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateCard()
    {
        
        var cardNumber = "1234567890123456";
        var hashedPin = "hashedPin123";
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var card = new Card(cardNumber, hashedPin, userId, accountId);

        card.CardNumber.Should().Be(cardNumber);
        card.HashedPin.Should().Be(hashedPin);
        card.UserId.Should().Be(userId);
        card.AccountId.Should().Be(accountId);
        card.Status.Should().Be(CardStatus.Active);
        card.FailedAttempts.Should().Be(0);
        card.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "hashedPin", "El número de tarjeta es requerido")]
    [InlineData(null, "hashedPin", "El número de tarjeta es requerido")]
    [InlineData("1234567890123456", "", "El PIN es requerido")]
    [InlineData("1234567890123456", null, "El PIN es requerido")]
    public void Constructor_InvalidParameters_ShouldThrowArgumentException(
        string? cardNumber, string? hashedPin, string expectedMessage)
    {
        
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        
        var action = () => new Card(cardNumber!, hashedPin!, userId, accountId);
        action.Should().Throw<ArgumentException>()
            .WithMessage($"{expectedMessage}*");
    }

    [Fact]
    public void ValidatePin_ValidPin_ShouldReturnTrueAndResetFailedAttempts()
    {
        var card = CreateValidCard();

        
        var result = card.ValidatePin(true);

        
        result.Should().BeTrue();
        card.FailedAttempts.Should().Be(0);
    }

    [Fact]
    public void ValidatePin_InvalidPin_ShouldIncrementFailedAttemptsAndThrowException()
    {
        var card = CreateValidCard();

        
        var action = () => card.ValidatePin(false);
        action.Should().Throw<InvalidPinException>()
            .WithMessage("PIN inválido. Intentos restantes: 3");

        card.FailedAttempts.Should().Be(1);
    }

    [Fact]
    public void ValidatePin_MaxFailedAttempts_ShouldBlockCardAndThrowCardBlockedException()
    {
        var card = CreateValidCard();

        
        for (int i = 0; i < 3; i++)
        {
            try { card.ValidatePin(false); } catch (InvalidPinException) { }
        }

        
        var action = () => card.ValidatePin(false);
        action.Should().Throw<CardBlockedException>();

        card.Status.Should().Be(CardStatus.Blocked);
    }

    [Fact]
    public void ValidatePin_BlockedCard_ShouldThrowCardBlockedException()
    {
        var card = CreateValidCard();
        
        
        for (int i = 0; i < 4; i++)
        {
            try { card.ValidatePin(false); } catch (Exception) { }
        }

        
        var action = () => card.ValidatePin(true);
        action.Should().Throw<CardBlockedException>();
    }

    [Fact]
    public void UnblockWithNewPin_ValidPin_ShouldUnblockCardAndSetNewPin()
    {
        var card = CreateValidCard();
        
        
        for (int i = 0; i < 4; i++)
        {
            try { card.ValidatePin(false); } catch (Exception) { }
        }

        var newHashedPin = "newHashedPin456";

        
        card.UnblockWithNewPin(newHashedPin);

        
        card.Status.Should().Be(CardStatus.Active);
        card.HashedPin.Should().Be(newHashedPin);
        card.FailedAttempts.Should().Be(0);
    }

    [Fact]
    public void UnblockWithNewPin_CardNotBlocked_ShouldThrowInvalidOperationException()
    {
        var card = CreateValidCard();

        
        var action = () => card.UnblockWithNewPin("newPin");
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Solo se pueden desbloquear tarjetas bloqueadas");
    }

    [Fact]
    public void SetCustomExpiryDate_ValidDate_ShouldSetExpiryDate()
    {
        var card = CreateValidCard();
        var futureDate = DateTime.UtcNow.AddYears(2);

        
        card.SetCustomExpiryDate(futureDate);

        
        card.ExpiryDate.Should().Be(futureDate);
    }

    [Fact]
    public void SetCustomExpiryDate_PastDate_ShouldThrowArgumentException()
    {
        var card = CreateValidCard();
        var pastDate = DateTime.UtcNow.AddDays(-1);

        
        var action = () => card.SetCustomExpiryDate(pastDate);
        action.Should().Throw<ArgumentException>()
            .WithMessage("La fecha de vencimiento debe ser mayor a la fecha actual*");
    }

    [Fact]
    public void IsActive_NewCard_ShouldReturnTrue()
    {
        var card = CreateValidCard();
        card.IsActive.Should().BeTrue();
    }

    private static Card CreateValidCard()
    {
        return new Card("1234567890123456", "hashedPin123", Guid.NewGuid(), Guid.NewGuid());
    }
}