using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class MailgunSettingsTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    MailgunSettingsModel model = new()
    {
      ApiKey = "api_key",
      DomainName = "domain_name"
    };

    MailgunSettings settings = new(model);

    Assert.Equal(model.ApiKey, settings.ApiKey);
    Assert.Equal(model.DomainName, settings.DomainName);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    string apiKey = "  api_key  ";
    string domainName = "  domain_name  ";

    MailgunSettings settings = new(apiKey, domainName);

    Assert.Equal(apiKey.Trim(), settings.ApiKey);
    Assert.Equal(domainName.Trim(), settings.DomainName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new MailgunSettings(string.Empty, "    "));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "ApiKey");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "DomainName");
  }
}
