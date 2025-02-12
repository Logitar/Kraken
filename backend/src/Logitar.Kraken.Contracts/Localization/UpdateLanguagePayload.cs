namespace Logitar.Kraken.Contracts.Localization;

public record UpdateLanguagePayload
{
  public string? Locale { get; set; }
}
