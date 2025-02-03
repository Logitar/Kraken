using Bogus;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Core;

[Trait(Traits.Category, Categories.Unit)]
public class ObjectExtensionsTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "DeepClone: it should clone deeply the object.")]
  public void Given_Object_When_DeepClone_Then_ClonedDeeply()
  {
    ContactInfo person = new()
    {
      Name = _faker.Person.FullName,
      Emails =
      [
        new EmailModel(_faker.Person.Email),
        new EmailModel(_faker.Internet.Email())
      ]
    };

    ContactInfo clone = person.DeepClone();
    Assert.Equal(person.Id, clone.Id);
    Assert.Equal(person.Name, clone.Name);
    Assert.True(person.Emails.SequenceEqual(clone.Emails));

    clone.Emails.ElementAt(0).IsVerified = true;
    Assert.False(person.Emails.ElementAt(0).IsVerified);
  }
}
