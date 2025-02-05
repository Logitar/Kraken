using Bogus;
using Logitar.Kraken.Contracts.Messages;

namespace Logitar.Kraken.Core.Messages;

[Trait(Traits.Category, Categories.Unit)]
public class RecipientsTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "It should construct the correct instance.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    Recipient to = new(RecipientType.To, _faker.Person.Email, _faker.Person.FullName);
    Recipient cc = new(RecipientType.CC, _faker.Internet.Email(), _faker.Name.FullName());
    Recipient bcc = new(RecipientType.Bcc, _faker.Internet.Email(), _faker.Name.FullName());

    Recipients recipients = new([to, cc, bcc]);

    Assert.Equal(to, Assert.Single(recipients.To));
    Assert.Equal(cc, Assert.Single(recipients.CC));
    Assert.Equal(bcc, Assert.Single(recipients.Bcc));

    int capacity = 3;
    Assert.Equal(capacity, (recipients.GetType().GetField("_to", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(recipients) as List<Recipient>)?.Capacity);
    Assert.Equal(capacity, (recipients.GetType().GetField("_cc", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(recipients) as List<Recipient>)?.Capacity);
    Assert.Equal(capacity, (recipients.GetType().GetField("_bcc", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(recipients) as List<Recipient>)?.Capacity);
  }
}
