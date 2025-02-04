namespace Logitar.Kraken.Contracts.Senders;

public record CreateOrReplaceSenderPayload
{
  public string? EmailAddress { get; set; }
  public string? PhoneNumber { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public MailgunSettingsModel? Mailgun { get; set; }
  public SendGridSettingsModel? SendGrid { get; set; }
  public TwilioSettingsModel? Twilio { get; set; }
}
