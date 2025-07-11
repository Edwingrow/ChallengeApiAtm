using ChallengeApiAtm.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Domain.Exceptions;

public class InsufficientFundsExceptionTests
{
    [Fact]
    public void Constructor_WithRequestedAndAvailableAmounts_ShouldSetCorrectMessage()
    {
        var requestedAmount = 500m;
        var availableAmount = 300m;

        var exception = new InsufficientFundsException(requestedAmount, availableAmount);

        var expectedMessage = $"Fondos insuficientes. Solicitado: {requestedAmount:C}, Disponible: {availableAmount:C}";
        exception.Message.Should().Be(expectedMessage);
        exception.RequestedAmount.Should().Be(requestedAmount);
        exception.AvailableBalance.Should().Be(availableAmount);
    }

    [Fact]
    public void Constructor_WithZeroAmounts_ShouldSetCorrectMessage()
    {
        var requestedAmount = 0m;
        var availableAmount = 0m;

        var exception = new InsufficientFundsException(requestedAmount, availableAmount);

        var expectedMessage = $"Fondos insuficientes. Solicitado: {requestedAmount:C}, Disponible: {availableAmount:C}";
        exception.Message.Should().Be(expectedMessage);
        exception.RequestedAmount.Should().Be(0);
        exception.AvailableBalance.Should().Be(0);
    }
}