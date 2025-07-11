using ChallengeApiAtm.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Domain.Exceptions;

public class CardBlockedExceptionTests
{
    [Fact]
    public void Constructor_WithCardNumber_ShouldSetCorrectMessage()
    {
        var cardNumber = "1234567890123456";

        var exception = new CardBlockedException(cardNumber);

        exception.Message.Should().Be($"La tarjeta {cardNumber} está bloqueada por múltiples intentos fallidos");
        exception.CardNumber.Should().Be(cardNumber);
    }

    [Fact]
    public void Constructor_WithEmptyCardNumber_ShouldSetMessage()
    {
        var cardNumber = "";

        var exception = new CardBlockedException(cardNumber);

       exception.Message.Should().Be("La tarjeta  está bloqueada por múltiples intentos fallidos");
        exception.CardNumber.Should().Be(cardNumber);
    }
}