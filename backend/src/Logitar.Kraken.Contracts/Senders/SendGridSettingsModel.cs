namespace Logitar.Kraken.Contracts.Senders;

public record SendGridSettingsModel : ISendGridSettings
{
  public string ApiKey { get; set; }

  public SendGridSettingsModel() : this(string.Empty)
  {
  }

  public SendGridSettingsModel(ISendGridSettings sendGrid) : this(sendGrid.ApiKey)
  {
  }

  public SendGridSettingsModel(string apiKey)
  {
    ApiKey = apiKey;
  }
}
