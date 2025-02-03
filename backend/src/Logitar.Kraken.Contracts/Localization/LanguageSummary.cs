namespace Logitar.Kraken.Contracts.Localization;

public class LanguageSummary
{
  public Guid Id { get; set; }
  public bool IsDefault { get; set; }
  public LocaleModel Locale { get; set; } = new();

  public LanguageSummary()
  {
  }

  public LanguageSummary(Guid id, LocaleModel locale, bool isDefault = false)
  {
    Id = id;
    Locale = locale;
    IsDefault = isDefault;
  }

  public override bool Equals(object? obj) => obj is LanguageSummary language && language.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Locale} (Id={Id})";
}
