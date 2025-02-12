namespace Logitar.Kraken.Contracts.Localization;

public record CreateOrReplaceLanguagePayload
{
  public string Locale { get; set; } = string.Empty;
}
