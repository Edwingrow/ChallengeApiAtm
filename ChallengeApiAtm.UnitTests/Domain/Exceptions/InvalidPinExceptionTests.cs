using ChallengeApiAtm.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Domain.Exceptions;

public class InvalidPinExceptionTests
{
    [Fact]
    public void Constructor_WithRemainingAttempts_ShouldSetCorrectMessage()
    {
        var remainingAttempts = 2;

        var exception = new InvalidPinException(remainingAttempts);

        exception.Message.Should().Be($"PIN inválido. Intentos restantes: {remainingAttempts}");
        exception.RemainingAttempts.Should().Be(remainingAttempts);
    }

    [Fact]
    public void Constructor_WithZeroAttempts_ShouldSetCorrectMessage()
    {
        var remainingAttempts = 0;

        var exception = new InvalidPinException(remainingAttempts);

        exception.Message.Should().Be("PIN inválido. Intentos restantes: 0");
        exception.RemainingAttempts.Should().Be(0);
    }
}