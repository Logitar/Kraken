using Logitar.Kraken.Contracts.Realms;

namespace Logitar.Kraken.Contracts.Senders;

public class SenderModel : AggregateModel
{
  public bool IsDefault { get; set; }

  public string? EmailAddress { get; set; }
  public string? PhoneNumber { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public SenderType Type { get; set; }
  public SenderProvider Provider { get; set; }
  public MailgunSettingsModel? Mailgun { get; set; }
  public SendGridSettingsModel? SendGrid { get; set; }
  public TwilioSettingsModel? Twilio { get; set; }

  public RealmModel? Realm { get; set; }
}
