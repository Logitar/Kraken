namespace Logitar.Kraken.Contracts.Senders;

public record TwilioSettingsModel : ITwilioSettings
{
  public string AccountSid { get; set; } = string.Empty;
  public string AuthenticationToken { get; set; } = string.Empty;

  public TwilioSettingsModel()
  {
  }

  public TwilioSettingsModel(ITwilioSettings twilio) : this(twilio.AccountSid, twilio.AuthenticationToken)
  {
  }

  public TwilioSettingsModel(string accountSid, string authenticationToken)
  {
    AccountSid = accountSid;
    AuthenticationToken = authenticationToken;
  }
}
