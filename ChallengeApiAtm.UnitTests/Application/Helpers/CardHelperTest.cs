using ChallengeApiAtm.Application.Helpers;
using FluentAssertions;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Application.Helpers;

public class CardHelperTest
{
    [Theory]
    [InlineData("1234567890123456", "****-****-****-3456")]
    [InlineData("1111222233334444", "****-****-****-4444")]
    [InlineData("9876543210123456", "****-****-****-3456")]
    public void MaskCardNumber_ValidCardNumber_ShouldReturnMaskedNumber(
        string cardNumber, string expected)
    {
        var result = CardHelper.MaskCardNumber(cardNumber);

        result.Should().Be(expected);
    }
}