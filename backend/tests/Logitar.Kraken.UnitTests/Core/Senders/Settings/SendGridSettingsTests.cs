using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class SendGridSettingsTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    SendGridSettingsModel model = new()
    {
      ApiKey = "api_key"
    };

    SendGridSettings settings = new(model);

    Assert.Equal(model.ApiKey, settings.ApiKey);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    string apiKey = "  api_key  ";

    SendGridSettings settings = new(apiKey);

    Assert.Equal(apiKey.Trim(), settings.ApiKey);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new SendGridSettings(string.Empty));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "ApiKey");
  }
}
