using Bogus;
using ChallengeApiAtm.Domain.Entities;

namespace ChallengeApiAtm.UnitTests.Builders;

/// <summary>
/// Builder para crear cuentas de prueba usando Bogus
/// </summary>
public class AccountBuilder
{
    private string _accountNumber = string.Empty;
    private Guid _userId = Guid.Empty;
    private decimal _initialBalance = 1000;

    public AccountBuilder()
    {
        var faker = new Faker();
        _accountNumber = faker.Random.String2(8, "0123456789");
        _userId = faker.Random.Guid();
        _initialBalance = faker.Random.Decimal(100, 10000);
    }

    public AccountBuilder WithAccountNumber(string accountNumber)
    {
        _accountNumber = accountNumber;
        return this;
    }

    public AccountBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public AccountBuilder WithInitialBalance(decimal initialBalance)
    {
        _initialBalance = initialBalance;
        return this;
    }

    public Account Build()
    {
        return new Account(_accountNumber, _userId, _initialBalance);
    }

    public List<Account> BuildMany(int count)
    {
        var accounts = new List<Account>();
        for (int i = 0; i < count; i++)
        {
            var faker = new Faker();
            var account = new Account(
                faker.Random.String2(8, "0123456789"),
                faker.Random.Guid(),
                faker.Random.Decimal(100, 10000)
            );
            accounts.Add(account);
        }
        return accounts;
    }
}