using ChallengeApiAtm.Application.DTOs.Requests;
using ChallengeApiAtm.Application.Interfaces;
using ChallengeApiAtm.Application.Services;
using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Interfaces;
using ChallengeApiAtm.UnitTests.Builders;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Application.Services;

/// <summary>
/// Tests for the UserService class
/// </summary>
public class UserServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<INumberGeneratorService> _numberGeneratorServiceMock;
    private readonly UserService _userService;

    public UserServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _cardRepositoryMock = new Mock<ICardRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _numberGeneratorServiceMock = new Mock<INumberGeneratorService>();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _accountRepositoryMock.Object,
            _cardRepositoryMock.Object,
            _passwordHasherMock.Object,
            _numberGeneratorServiceMock.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_ValidRequest_ShouldReturnRegisterResponse()
    {
        var request = new RegisterRequest
         {
        FirstName = "Edwin",
        LastName = "Garcia",
        DocumentNumber = "96069288",
        CardNumber = "4001234567890101",
        ExpiryDate = "12/2028",
        Pin = "8033",
        ConfirmPin = "8033",
        InitialBalance = 1500000
    };

        var accountNumber = "1420284881";
        var hashedPin = "hashedPin8033";

        _userRepositoryMock
            .Setup(x => x.GetByDocumentNumberAsync(request.DocumentNumber, default))
            .ReturnsAsync((User?)null);

        _cardRepositoryMock
            .Setup(x => x.ExistsByCardNumberAsync(request.CardNumber, default))
            .ReturnsAsync(false);

        _numberGeneratorServiceMock
            .Setup(x => x.GenerateUniqueAccountNumberAsync(default))
            .ReturnsAsync(accountNumber);

        _passwordHasherMock
            .Setup(x => x.HashPin(request.Pin))
            .Returns(hashedPin);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>(), default))
            .Returns(Task.CompletedTask);

        _accountRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Account>(), default))
            .Returns(Task.CompletedTask);

        _cardRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Card>(), default))
            .Returns(Task.CompletedTask);

        var result = await _userService.RegisterUserAsync(request);

        result.Should().NotBeNull();
        result.FirstName.Should().Be(request.FirstName);
        result.LastName.Should().Be(request.LastName);
        result.AccountNumber.Should().Be(accountNumber);
        result.CardNumber.Should().Be("****-****-****-0101"); 
        result.InitialBalance.Should().Be(request.InitialBalance);
        result.ExpiryDate.Should().Be(request.ExpiryDate);
        result.Message.Should().Be("Usuario registrado exitosamente");

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), default), Times.Once);
        _accountRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Account>(), default), Times.Once);
        _cardRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Card>(), default), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_ExistingUser_ShouldThrowInvalidOperationException()
    {
        var request = new RegisterRequest
        {
            DocumentNumber = "96069288",
            CardNumber = "4001234567890101",
            ExpiryDate = "12/2028",
            Pin = "8033",
            ConfirmPin = "8033",
            InitialBalance = 1500000,
            FirstName = "Edwin",
            LastName = "Garcia"
        };

        var existingUser = new UserBuilder()
            .WithDocumentNumber(request.DocumentNumber)
            .Build();

        _userRepositoryMock
            .Setup(x => x.GetByDocumentNumberAsync(request.DocumentNumber, default))
            .ReturnsAsync(existingUser);

        var action = async () => await _userService.RegisterUserAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Ya existe un usuario registrado con este número de documento");
    }

    [Fact]
    public async Task RegisterUserAsync_ExistingCard_ShouldThrowInvalidOperationException()
    {
        var request = new RegisterRequest
        {
            DocumentNumber = "96069288",
            CardNumber = "4001234567890101",
            ExpiryDate = "12/2028",
            Pin = "8033",
            ConfirmPin = "8033",
            InitialBalance = 1500000,
            FirstName = "Edwin",
            LastName = "Garcia"
        };

        _userRepositoryMock
            .Setup(x => x.GetByDocumentNumberAsync(request.DocumentNumber, default))
            .ReturnsAsync((User?)null);

        _cardRepositoryMock
            .Setup(x => x.ExistsByCardNumberAsync(request.CardNumber, default))
            .ReturnsAsync(true);

        var action = async () => await _userService.RegisterUserAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Ya existe una tarjeta con este número");
    }

    [Fact]
    public async Task RegisterUserAsync_InvalidExpiryDate_ShouldThrowInvalidOperationException()
    {
        var request = new RegisterRequest
        {
            DocumentNumber = "96069288",
            CardNumber = "4001234567890101",
            ExpiryDate = "01/2020",
            Pin = "8033",
            ConfirmPin = "8033",
            InitialBalance = 1500000,
            FirstName = "Edwin",
            LastName = "Garcia"
        };

        _userRepositoryMock
            .Setup(x => x.GetByDocumentNumberAsync(request.DocumentNumber, default))
            .ReturnsAsync((User?)null);

        _cardRepositoryMock
            .Setup(x => x.ExistsByCardNumberAsync(request.CardNumber, default))
            .ReturnsAsync(false);

        var action = async () => await _userService.RegisterUserAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("La fecha de vencimiento debe ser mayor a la fecha actual");
    }

    [Fact]
    public async Task RegisterUserAsync_InvalidExpiryDateFormat_ShouldThrowInvalidOperationException()
    {
        var request = new RegisterRequest
        {
            DocumentNumber = "96069288",
            CardNumber = "4001234567890101",
            ExpiryDate = "invalid-date",
            Pin = "8033",
            ConfirmPin = "8033",
            InitialBalance = 1500000,
            FirstName = "Edwin",
            LastName = "Garcia"
        };

        _userRepositoryMock
            .Setup(x => x.GetByDocumentNumberAsync(request.DocumentNumber, default))
            .ReturnsAsync((User?)null);

        _cardRepositoryMock
            .Setup(x => x.ExistsByCardNumberAsync(request.CardNumber, default))
            .ReturnsAsync(false);
        var action = async () => await _userService.RegisterUserAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Formato de fecha de vencimiento inválido");
    }
}