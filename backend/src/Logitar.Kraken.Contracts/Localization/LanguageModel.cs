namespace Logitar.Kraken.Contracts.Localization;

public class LanguageModel : AggregateModel
{
  public bool IsDefault { get; set; }

  public LocaleModel Locale { get; set; } = new();

  public LanguageModel()
  {
  }

  public LanguageModel(LocaleModel locale, bool isDefault = false)
  {
    IsDefault = isDefault;

    Locale = locale;
  }

  public override string ToString() => $"{Locale} | {base.ToString()}";
}
