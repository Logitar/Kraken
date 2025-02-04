using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.Settings;

public record TwilioSettings : SenderSettings, ITwilioSettings
{
  [JsonIgnore]
  public override SenderProvider Provider { get; } = SenderProvider.Twilio;

  public string AccountSid { get; }
  public string AuthenticationToken { get; }

  [JsonConstructor]
  public TwilioSettings(string accountSid, string authenticationToken)
  {
    AccountSid = accountSid.Trim();
    AuthenticationToken = authenticationToken.Trim();
    new TwilioSettingsValidator().ValidateAndThrow(this);
  }

  public TwilioSettings(ITwilioSettings twilio) : this(twilio.AccountSid, twilio.AuthenticationToken)
  {
  }
}
