namespace Logitar.Kraken.Contracts.Senders;

public interface ITwilioSettings
{
  string AccountSid { get; }
  string AuthenticationToken { get; }
}
