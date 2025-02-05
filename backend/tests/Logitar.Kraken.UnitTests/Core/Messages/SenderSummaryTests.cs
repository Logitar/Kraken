using Bogus;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.Core.Senders.Settings;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Messages;

[Trait(Traits.Category, Categories.Unit)]
public class SenderSummaryTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "It should construct the correct instance from a sender.")]
  public void Given_Sender_When_ctor_Then_Constructed()
  {
    Phone phone = new("+15148454636");
    TwilioSettings settings = new("AccountSid", "AuthenticationToken");
    Sender sender = new(email: null, phone, settings);

    SenderSummary summary = new(sender);

    Assert.Equal(sender.Id, summary.Id);
    Assert.Equal(sender.IsDefault, summary.IsDefault);
    Assert.Equal(sender.Email, summary.Email);
    Assert.Equal(sender.Phone, summary.Phone);
    Assert.Equal(sender.DisplayName, summary.DisplayName);
    Assert.Equal(sender.Provider, summary.Provider);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    SenderId id = SenderId.NewId(realmId: null);
    bool isDefault = true;
    Email email = new(_faker.Person.Email);
    Phone? phone = null;
    DisplayName displayName = new(_faker.Person.FullName);
    SenderProvider provider = SenderProvider.SendGrid;

    SenderSummary sender = new(id, isDefault, email, phone, displayName, provider);

    Assert.Equal(id, sender.Id);
    Assert.Equal(isDefault, sender.IsDefault);
    Assert.Equal(email, sender.Email);
    Assert.Equal(phone, sender.Phone);
    Assert.Equal(displayName, sender.DisplayName);
    Assert.Equal(provider, sender.Provider);
  }
}
