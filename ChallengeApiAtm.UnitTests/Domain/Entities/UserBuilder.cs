using Bogus;
using ChallengeApiAtm.Domain.Entities;

namespace ChallengeApiAtm.UnitTests.Builders;

/// <summary>
/// Builder para crear usuarios de prueba usando Bogus
/// </summary>
public class UserBuilder
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _documentNumber = string.Empty;

    public UserBuilder()
    {
        var faker = new Faker();
        _firstName = faker.Person.FirstName;
        _lastName = faker.Person.LastName;
        _documentNumber = faker.Random.String2(8, "0123456789");
    }

    public UserBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserBuilder WithDocumentNumber(string documentNumber)
    {
        _documentNumber = documentNumber;
        return this;
    }

    public User Build()
    {
        return new User(_firstName, _lastName, _documentNumber);
    }

    public List<User> BuildMany(int count)
    {
        var users = new List<User>();
        for (int i = 0; i < count; i++)
        {
            var faker = new Faker();
            var user = new User(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Random.String2(8, "0123456789")
            );
            users.Add(user);
        }
        return users;
    }
}