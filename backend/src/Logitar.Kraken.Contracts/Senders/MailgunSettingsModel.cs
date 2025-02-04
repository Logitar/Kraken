namespace Logitar.Kraken.Contracts.Senders;

public record MailgunSettingsModel : IMailgunSettings
{
  public string ApiKey { get; set; } = string.Empty;

  public string DomainName { get; set; } = string.Empty;

  public MailgunSettingsModel()
  {
  }

  public MailgunSettingsModel(IMailgunSettings mailgun) : this(mailgun.ApiKey, mailgun.DomainName)
  {
  }

  public MailgunSettingsModel(string apiKey, string domainName)
  {
    ApiKey = apiKey;
    DomainName = domainName;
  }
}
