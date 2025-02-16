namespace Logitar.Kraken.Contracts.Localization;

public class LocaleModel
{
  public int Id { get; set; }
  public string Code { get; set; }
  public string DisplayName { get; set; }
  public string EnglishName { get; set; }
  public string NativeName { get; set; }

  public LocaleModel() : this(string.Empty)
  {
  }

  public LocaleModel(string cultureName) : this(CultureInfo.GetCultureInfo(cultureName.Trim()))
  {
  }

  public LocaleModel(CultureInfo culture)
  {
    Id = culture.LCID;
    Code = culture.Name;
    DisplayName = culture.DisplayName;
    EnglishName = culture.EnglishName;
    NativeName = culture.NativeName;
  }

  public override bool Equals(object? obj) => obj is LocaleModel locale && locale.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName} ({Code})";
}
