using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.Settings;

public record MailgunSettings : SenderSettings, IMailgunSettings
{
  [JsonIgnore]
  public override SenderProvider Provider { get; } = SenderProvider.Mailgun;

  public string ApiKey { get; }
  public string DomainName { get; }

  [JsonConstructor]
  public MailgunSettings(string apiKey, string domainName)
  {
    ApiKey = apiKey.Trim();
    DomainName = domainName.Trim();
    new MailgunSettingsValidator().ValidateAndThrow(this);
  }

  public MailgunSettings(IMailgunSettings mailgun) : this(mailgun.ApiKey, mailgun.DomainName)
  {
  }
}
