
using ChallengeApiAtm.Application.Services;
using ChallengeApiAtm.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Application.Services;

/// <summary>
/// Tests for the NumberGeneratorService class
/// </summary>
public class NumberGeneratorServiceTest
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly NumberGeneratorService _numberGeneratorService;

    public NumberGeneratorServiceTest()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _cardRepositoryMock = new Mock<ICardRepository>();
        _numberGeneratorService = new NumberGeneratorService(
            _accountRepositoryMock.Object,
            _cardRepositoryMock.Object);
    }

    [Fact]
    public async Task GenerateUniqueAccountNumberAsync_ShouldGenerateValidAccountNumber()
        {
        _accountRepositoryMock
            .Setup(x => x.ExistsByAccountNumberAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);

        
        var result = await _numberGeneratorService.GenerateUniqueAccountNumberAsync();

        
        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex(@"^\d{10}$"); 
        
        _accountRepositoryMock.Verify(
            x => x.ExistsByAccountNumberAsync(result, default), 
            Times.Once);
    }

    [Fact]
    public async Task GenerateUniqueAccountNumberAsync_ShouldRetryWhenNumberExists()
    {
        
        var sequence = new MockSequence();
        
        
        _accountRepositoryMock.InSequence(sequence)
            .Setup(x => x.ExistsByAccountNumberAsync(It.IsAny<string>(), default))
            .ReturnsAsync(true);
        
        
        _accountRepositoryMock.InSequence(sequence)
            .Setup(x => x.ExistsByAccountNumberAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);

        
        var result = await _numberGeneratorService.GenerateUniqueAccountNumberAsync();

        
        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex(@"^\d{10}$");
        
        
        _accountRepositoryMock.Verify(
            x => x.ExistsByAccountNumberAsync(It.IsAny<string>(), default), 
            Times.AtLeast(2));
    }

    [Fact]
    public async Task GenerateUniqueAccountNumberAsync_ShouldGenerateNumberInValidRange()
    {
        
        _accountRepositoryMock
            .Setup(x => x.ExistsByAccountNumberAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);

        
        var result = await _numberGeneratorService.GenerateUniqueAccountNumberAsync();

        
        var numberValue = long.Parse(result);
        
        
        numberValue.Should().BeGreaterThanOrEqualTo(1000000000L); 
        numberValue.Should().BeLessThanOrEqualTo(int.MaxValue);  
    }

    [Fact]
    public async Task GenerateUniqueAccountNumberAsync_ShouldAlwaysCallRepository()
    {
        
        _accountRepositoryMock
            .Setup(x => x.ExistsByAccountNumberAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);

        
        await _numberGeneratorService.GenerateUniqueAccountNumberAsync();

        
        _accountRepositoryMock.Verify(
            x => x.ExistsByAccountNumberAsync(It.IsAny<string>(), default), 
            Times.Once);
    }

    [Fact]
    public async Task GenerateUniqueAccountNumberAsync_MultipleCalls_ShouldGenerateDifferentNumbers()
    {
        
        _accountRepositoryMock
            .Setup(x => x.ExistsByAccountNumberAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);

        
        var results = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            var number = await _numberGeneratorService.GenerateUniqueAccountNumberAsync();
            results.Add(number);
        }

        
        results.Should().OnlyHaveUniqueItems(); 
        results.Should().HaveCount(10);
        
       
        results.Should().AllSatisfy(x => x.Should().MatchRegex(@"^\d{10}$"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task GenerateUniqueAccountNumberAsync_ShouldRetryMultipleTimes(int retryCount)
    {
        var callCount = 0;
        _accountRepositoryMock
            .Setup(x => x.ExistsByAccountNumberAsync(It.IsAny<string>(), default))
            .ReturnsAsync(() => 
            {
                callCount++;
                return callCount <= retryCount;
            });

       
        var result = await _numberGeneratorService.GenerateUniqueAccountNumberAsync();

        
        result.Should().NotBeNullOrEmpty();
        
       
        _accountRepositoryMock.Verify(
            x => x.ExistsByAccountNumberAsync(It.IsAny<string>(), default), 
            Times.Exactly(retryCount + 1));
    }
}