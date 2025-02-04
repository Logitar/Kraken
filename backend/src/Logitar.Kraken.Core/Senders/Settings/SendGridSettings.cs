using FluentValidation;
using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders.SendGrid;

public record SendGridSettings : SenderSettings, ISendGridSettings
{
  [JsonIgnore]
  public override SenderProvider Provider { get; } = SenderProvider.SendGrid;

  public string ApiKey { get; }

  [JsonConstructor]
  public SendGridSettings(string apiKey)
  {
    ApiKey = apiKey.Trim();
    new SendGridSettingsValidator().ValidateAndThrow(this);
  }

  public SendGridSettings(ISendGridSettings sendGrid) : this(sendGrid.ApiKey)
  {
  }
}
