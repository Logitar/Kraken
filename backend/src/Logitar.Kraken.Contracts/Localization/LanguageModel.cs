namespace Logitar.Kraken.Contracts.Localization;

public class LanguageModel : AggregateModel
{
  public bool IsDefault { get; set; }

  public LocaleModel Locale { get; set; } = new();

  public override string ToString() => $"{Locale} | {base.ToString()}";
}
