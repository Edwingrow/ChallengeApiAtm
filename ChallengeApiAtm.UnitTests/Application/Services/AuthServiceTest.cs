using ChallengeApiAtm.Application.DTOs.Requests;
using ChallengeApiAtm.Application.DTOs.Responses;
using ChallengeApiAtm.Application.Interfaces;
using ChallengeApiAtm.Application.Services;
using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Enums;
using ChallengeApiAtm.Domain.Exceptions;
using ChallengeApiAtm.Domain.Interfaces;
using ChallengeApiAtm.UnitTests.Builders;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Application.Services;

/// <summary>
/// Tests for the AuthService class
/// </summary>
public class AuthServiceTest
{
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTest()
    {
        _cardRepositoryMock = new Mock<ICardRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _authService = new AuthService(
            _cardRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object);
    }
    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnLoginResponse()
    {
        var scenario = new TestScenarioBuilder()
            .WithCardNumber("1234567890123456")
            .WithInitialBalance(1000);

        var (user, account, card) = scenario.BuildCompleteScenario();

        var request = new LoginRequest
        {
            CardNumber = card.CardNumber,
            Pin = "1234"
        };

        var expectedToken = "jwt-token-123";
        var expectedExpirationSeconds = 3600;

        _cardRepositoryMock
           .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
           .ReturnsAsync(card);

        _passwordHasherMock
            .Setup(x => x.VerifyPin(request.Pin, card.HashedPin))
            .Returns(true);

        _jwtTokenServiceMock
            .Setup(x => x.GenerateToken(card.UserId, card.CardNumber, card.Account.AccountNumber))
            .Returns(expectedToken);

        _jwtTokenServiceMock
            .Setup(x => x.GetTokenExpirationInSeconds())
            .Returns(expectedExpirationSeconds);

        _cardRepositoryMock
            .Setup(x => x.UpdateAsync(card, default))
            .Returns(Task.CompletedTask);

        var result = await _authService.LoginAsync(request, default);

        result.Should().NotBeNull();
        result.Token.Should().Be(expectedToken);
        result.TokenType.Should().Be("Bearer");
        result.ExpiresIn.Should().Be(expectedExpirationSeconds);
        result.UserInfo.Should().NotBeNull();
        result.UserInfo.FullName.Should().Be(user.FullName);
        result.UserInfo.AccountNumber.Should().Be(account.AccountNumber);
        result.UserInfo.CardNumber.Should().Be("****-****-****-3456");

        _cardRepositoryMock.Verify(x => x.UpdateAsync(card, default), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_CardNotFound_ShouldThrowInvalidPinException()
    {
        var request = new LoginRequest
        {
            CardNumber = "9999999999999999",
            Pin = "1234"
        };
        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync((Card?)null);

        var action = async () => await _authService.LoginAsync(request);
        await action.Should().ThrowAsync<InvalidPinException>();
    }

    [Fact]
    public async Task LoginAsync_InvalidPin_ShouldThrowInvalidPinException()
    {
        var scenario = new TestScenarioBuilder()
            .WithCardNumber("1234567890123456")
            .WithHashedPin("hashedPin123");

        var (user, account, card) = scenario.BuildCompleteScenario();

        var request = new LoginRequest
        {
            CardNumber = card.CardNumber,
            Pin = "5555"
        };
        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync(card);
        _passwordHasherMock
                    .Setup(x => x.VerifyPin(request.Pin, card.HashedPin))
                    .Returns(false);

        _cardRepositoryMock
           .Setup(x => x.UpdateAsync(card, default))
           .Returns(Task.CompletedTask);

        var action = async () => await _authService.LoginAsync(request);
        await action.Should().ThrowAsync<InvalidPinException>();

        _cardRepositoryMock.Verify(x => x.UpdateAsync(card, default), Times.Once);
    }
    [Fact]
    public async Task LoginAsync_CardBlocked_ShouldThrowCardBlockedException()
    {
        var scenario = new TestScenarioBuilder()
            .WithCardNumber("1234567890123456")
            .WithHashedPin("hashedPin123");

        var (user, account, card) = scenario.BuildCompleteScenario();


       typeof(Card).GetProperty("FailedAttempts")?.SetValue(card, 4);

        var request = new LoginRequest
        {
            CardNumber = card.CardNumber,
            Pin = "5555"
        };

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync(card);

        _passwordHasherMock
            .Setup(x => x.VerifyPin(request.Pin, card.HashedPin))
            .Returns(false);

        _cardRepositoryMock
            .Setup(x => x.UpdateAsync(card, default))
            .Returns(Task.CompletedTask);


        var action = async () => await _authService.LoginAsync(request);
        await action.Should().ThrowAsync<CardBlockedException>();

        _cardRepositoryMock.Verify(x => x.UpdateAsync(card, default), Times.Once);
    }

    [Fact]
    public async Task GetUserInfoFromTokenAsync_ValidToken_ShouldReturnUserInfo()
    {

        var token = "valid-jwt-token";
        var cardNumber = "1234567890123456";

        var scenario = new TestScenarioBuilder()
            .WithCardNumber(cardNumber);

        var (user, account, card) = scenario.BuildCompleteScenario();

        _jwtTokenServiceMock
            .Setup(x => x.GetCardNumberFromToken(token))
            .Returns(cardNumber);

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(cardNumber, default))
            .ReturnsAsync(card);


        var result = await _authService.GetUserInfoFromTokenAsync(token);


        result.Should().NotBeNull();
        result!.FullName.Should().Be(user.FullName);
        result.AccountNumber.Should().Be(account.AccountNumber);
        result.CardNumber.Should().Be("****-****-****-3456");
    }

    [Fact]
    public async Task GetUserInfoFromTokenAsync_InvalidToken_ShouldReturnNull()
    {

        var invalidToken = "invalid-token";

        _jwtTokenServiceMock
            .Setup(x => x.GetCardNumberFromToken(invalidToken))
            .Returns((string?)null);


        var result = await _authService.GetUserInfoFromTokenAsync(invalidToken);


        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserInfoFromTokenAsync_CardNotFound_ShouldReturnNull()
    {

        var token = "valid-token";
        var cardNumber = "9999999999999999";

        _jwtTokenServiceMock
            .Setup(x => x.GetCardNumberFromToken(token))
            .Returns(cardNumber);

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(cardNumber, default))
            .ReturnsAsync((Card?)null);


        var result = await _authService.GetUserInfoFromTokenAsync(token);


        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserInfoFromTokenAsync_ExceptionThrown_ShouldReturnNull()
    {

        var token = "problematic-token";

        _jwtTokenServiceMock
            .Setup(x => x.GetCardNumberFromToken(token))
            .Throws(new ArgumentException("Token malformed"));


        var result = await _authService.GetUserInfoFromTokenAsync(token);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UnblockCardAsync_ValidRequest_ShouldUnblockCard()
    {

        var scenario = new TestScenarioBuilder()
            .WithCardNumber("1234567890123456");

        var (user, account, card) = scenario.BuildCompleteScenario();


        typeof(Card).GetProperty("Status")?.SetValue(card, CardStatus.Blocked);

        var request = new UnblockCardRequest
        {
            CardNumber = card.CardNumber,
            DocumentNumber = user.DocumentNumber,
            NewPin = "5678",
            ConfirmNewPin = "5678"
        };

        var hashedNewPin = "newHashedPin789";

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync(card);

        _passwordHasherMock
            .Setup(x => x.HashPin(request.NewPin))
            .Returns(hashedNewPin);

        _cardRepositoryMock
            .Setup(x => x.UpdateAsync(card, default))
            .Returns(Task.CompletedTask);

        var result = await _authService.UnblockCardAsync(request);

        result.Should().NotBeNull();
        result.CardNumber.Should().Be("****-****-****-3456");
        result.CardHolderName.Should().Be(user.FullName);
        result.CardStatus.Should().Be("Activa");
        result.Message.Should().Be("Tarjeta desbloqueada exitosamente");
        result.UnblockDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        _cardRepositoryMock.Verify(x => x.UpdateAsync(card, default), Times.Once);
    }

    [Fact]
    public async Task UnblockCardAsync_CardNotFound_ShouldThrowInvalidOperationException()
    {

        var request = new UnblockCardRequest
        {
            CardNumber = "9999999999999999",
            DocumentNumber = "12345678",
            NewPin = "5678",
            ConfirmNewPin = "5678"
        };

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync((Card?)null);


        var action = async () => await _authService.UnblockCardAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Tarjeta no encontrada");
    }

    [Fact]
    public async Task UnblockCardAsync_DocumentMismatch_ShouldThrowInvalidOperationException()
    {

        var scenario = new TestScenarioBuilder()
            .WithCardNumber("1234567890123456");

        var (user, account, card) = scenario.BuildCompleteScenario();

        var request = new UnblockCardRequest
        {
            CardNumber = card.CardNumber,
            DocumentNumber = "99999999",
            NewPin = "5678",
            ConfirmNewPin = "5678"
        };

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync(card);


        var action = async () => await _authService.UnblockCardAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("El número de documento no coincide con el titular de la tarjeta");
    }

    [Fact]
    public async Task UnblockCardAsync_CardNotBlocked_ShouldThrowInvalidOperationException()
    {

        var scenario = new TestScenarioBuilder()
            .WithCardNumber("1234567890123456");

        var (user, account, card) = scenario.BuildCompleteScenario();


        typeof(Card).GetProperty("Status")?.SetValue(card, CardStatus.Active);

        var request = new UnblockCardRequest
        {
            CardNumber = card.CardNumber,
            DocumentNumber = user.DocumentNumber,
            NewPin = "5678",
            ConfirmNewPin = "5678"
        };

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync(card);


        var action = async () => await _authService.UnblockCardAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("La tarjeta no está bloqueada");
    }


}