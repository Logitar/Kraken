namespace Logitar.Kraken.Contracts.Senders;

public record UpdateSenderPayload
{
  public string? EmailAddress { get; set; }
  public string? PhoneNumber { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }

  public MailgunSettingsModel? Mailgun { get; set; }
  public SendGridSettingsModel? SendGrid { get; set; }
  public TwilioSettingsModel? Twilio { get; set; }
}
