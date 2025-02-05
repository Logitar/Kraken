using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders;

[Trait(Traits.Category, Categories.Unit)]
public class SenderProviderExtensionsTests
{
  private readonly Dictionary<SenderProvider, SenderType> _mapping = new()
  {
    [SenderProvider.Mailgun] = SenderType.Email,
    [SenderProvider.SendGrid] = SenderType.Email,
    [SenderProvider.Twilio] = SenderType.Phone
  };

  [Fact(DisplayName = "GetSenderType: it should return the correct sender type given a sender provider.")]
  public void Given_SenderProvider_When_GetSenderType_Then_SenderType()
  {
    foreach (SenderProvider provider in Enum.GetValues<SenderProvider>())
    {
      Assert.Equal(_mapping[provider], provider.GetSenderType());
    }
  }

  [Fact(DisplayName = "GetSenderType: it should throw SenderProviderNotSupportedException when the sender provider is not supported.")]
  public void Given_NotSupported_When_GetSenderType_Then_SenderProviderNotSupportedException()
  {
    SenderProvider provider = (SenderProvider)(-1);
    var exception = Assert.Throws<SenderProviderNotSupportedException>(() => provider.GetSenderType());
    Assert.Equal(provider, exception.SenderProvider);
  }

  [Fact(DisplayName = "IsEmailSender: it should return the correct boolean value given a sender provider.")]
  public void Given_SenderProvider_When_IsEmailSender_Then_CorrectBoolean()
  {
    foreach (SenderProvider provider in Enum.GetValues<SenderProvider>())
    {
      bool isEmail = _mapping[provider] == SenderType.Email;
      Assert.Equal(isEmail, provider.IsEmailSender());
    }
  }

  [Fact(DisplayName = "IsPhoneSender: it should return the correct boolean value given a sender provider.")]
  public void Given_SenderProvider_When_IsPhoneSender_Then_CorrectBoolean()
  {
    foreach (SenderProvider provider in Enum.GetValues<SenderProvider>())
    {
      bool isEmail = _mapping[provider] == SenderType.Phone;
      Assert.Equal(isEmail, provider.IsPhoneSender());
    }
  }
}
