using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class TwilioSettingsTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    TwilioSettingsModel model = new()
    {
      AccountSid = "account_sid",
      AuthenticationToken = "auth_token"
    };

    TwilioSettings settings = new(model);

    Assert.Equal(model.AccountSid, settings.AccountSid);
    Assert.Equal(model.AuthenticationToken, settings.AuthenticationToken);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    string accountSid = "  account_sid  ";
    string authenticationToken = "  auth_token  ";

    TwilioSettings settings = new(accountSid, authenticationToken);

    Assert.Equal(accountSid.Trim(), settings.AccountSid);
    Assert.Equal(authenticationToken.Trim(), settings.AuthenticationToken);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new TwilioSettings(string.Empty, "    "));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "AccountSid");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "AuthenticationToken");
  }
}
