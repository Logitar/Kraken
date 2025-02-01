namespace Logitar.Kraken.Contracts.ApiKeys;

public record AuthenticateApiKeyPayload
{
  public string XApiKey { get; set; } = string.Empty;
}
