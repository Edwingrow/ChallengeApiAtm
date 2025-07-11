using Bogus;
using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Enums;

namespace ChallengeApiAtm.UnitTests.Builders;


/// <summary>
/// Builder para crear transacciones de prueba usando Bogus
/// </summary>
public class TransactionBuilder
{
    private readonly Faker _faker = new Faker();
    private Guid _accountId = Guid.Empty;
    private Guid _cardId = Guid.Empty;
    private TransactionType _type = TransactionType.Withdrawal;
    private decimal _amount = 0;
    private string _description = string.Empty;

    public TransactionBuilder()
    {
        _accountId = _faker.Random.Guid();
        _cardId = _faker.Random.Guid();
        _type = _faker.PickRandom<TransactionType>();
        _amount = _faker.Random.Decimal(1, 1000);
        _description = _faker.Lorem.Sentence();
    }
    public TransactionBuilder WithAccountId(Guid accountId)
    {
        _accountId = accountId;
        return this;
    }

    public TransactionBuilder WithCardId(Guid cardId)
    {
        _cardId = cardId;
        return this;
    }

    public TransactionBuilder WithType(TransactionType type)
    {
        _type = type;
        return this;
    }

    public TransactionBuilder WithAmount(decimal amount)
    
    {
        _amount = amount;
        return this;
    }

    public TransactionBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public Transaction Build()
    {
        return new Transaction(_accountId, _cardId, _type, _amount, _description);
    }

}   