using Bogus;
using ChallengeApiAtm.Domain.Entities;

namespace ChallengeApiAtm.UnitTests.Builders;

/// <summary>
/// Builder para crear tarjetas de prueba usando Bogus
/// </summary>
public class CardBuilder
{
    private string _cardNumber = string.Empty;
    private string _hashedPin = string.Empty;
    private Guid _userId = Guid.Empty;
    private Guid _accountId = Guid.Empty;

    public CardBuilder()
    {
        var faker = new Faker();
        _cardNumber = faker.Random.String2(16, "0123456789");
        _hashedPin = faker.Random.String2(64); 
        _userId = faker.Random.Guid();
        _accountId = faker.Random.Guid();
    }

    public CardBuilder WithCardNumber(string cardNumber)
    {
        _cardNumber = cardNumber;
        return this;
    }

    public CardBuilder WithHashedPin(string hashedPin)
    {
        _hashedPin = hashedPin;
        return this;
    }

    public CardBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public CardBuilder WithAccountId(Guid accountId)
    {
        _accountId = accountId;
        return this;
    }

    public Card Build()
    {
        return new Card(_cardNumber, _hashedPin, _userId, _accountId);
    }

    public List<Card> BuildMany(int count)
    {
        var cards = new List<Card>();
        for (int i = 0; i < count; i++)
        {
            var faker = new Faker();
            var card = new Card(
                faker.Random.String2(16, "0123456789"),
                faker.Random.String2(64),
                faker.Random.Guid(),
                faker.Random.Guid()
            );
            cards.Add(card);
        }
        return cards;
    }
}