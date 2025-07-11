using ChallengeApiAtm.Application.DTOs.Requests;
using ChallengeApiAtm.Domain.Entities;
using Bogus;

namespace ChallengeApiAtm.UnitTests.Builders;

/// <summary>
/// Builder para crear escenarios completos de test con entidades relacionadas
/// </summary>
public class TestScenarioBuilder
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _documentNumber = string.Empty;
    private string _cardNumber = string.Empty;
    private string _hashedPin = string.Empty;
    private string _accountNumber = string.Empty;
    private decimal _initialBalance = 1000;

    public TestScenarioBuilder()
    {
        var faker = new Faker();
        _firstName = faker.Person.FirstName;
        _lastName = faker.Person.LastName;
        _documentNumber = faker.Random.String2(8, "0123456789");
        _cardNumber = faker.Random.String2(16, "0123456789");
        _hashedPin = faker.Random.String2(64);
        _accountNumber = faker.Random.String2(8, "0123456789");
        _initialBalance = faker.Random.Decimal(500, 5000);
    }

    public TestScenarioBuilder WithInitialBalance(decimal balance)
    {
        _initialBalance = balance;
        return this;
    }

    public TestScenarioBuilder WithCardNumber(string cardNumber)
    {
        _cardNumber = cardNumber;
        return this;
    }

    public TestScenarioBuilder WithHashedPin(string hashedPin)
    {
        _hashedPin = hashedPin;
        return this;
    }
    public (User user, Account account, Card card) BuildCompleteScenario()
    {
        var user = new UserBuilder()
            .WithFirstName(_firstName)
            .WithLastName(_lastName)
            .WithDocumentNumber(_documentNumber)
            .Build();

        var account = new AccountBuilder()
            .WithAccountNumber(_accountNumber)
            .WithUserId(user.Id)
            .WithInitialBalance(_initialBalance)
            .Build();

        var card = new CardBuilder()
            .WithCardNumber(_cardNumber)
            .WithHashedPin(_hashedPin)
            .WithUserId(user.Id)
            .WithAccountId(account.Id)
            .Build();

        SetNavigationProperty(card, "User", user);
        SetNavigationProperty(card, "Account", account);

        return (user, account, card);
    }

   
    private static void SetNavigationProperty(object entity, string propertyName, object value)
    {
        var property = entity.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(entity, value);
        }
        else
        {
            var backingField = entity.GetType().GetField($"<{propertyName}>k__BackingField", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            backingField?.SetValue(entity, value);
        }
    }

    
    public RegisterRequest BuildRegisterRequest(string plainTextPin = "1234")
    {
        return new RegisterRequest
        {
            FirstName = _firstName,
            LastName = _lastName,
            DocumentNumber = _documentNumber,
            CardNumber = _cardNumber,
            Pin = plainTextPin,
            ConfirmPin = plainTextPin,
            InitialBalance = _initialBalance
        };
    }
    
}