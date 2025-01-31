namespace Logitar.Kraken.Contracts.Localization;

public record LocaleModel
{
  public int LCID { get; set; }
  public string Code { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
  public string EnglishName { get; set; } = string.Empty;
  public string NativeName { get; set; } = string.Empty;

  public LocaleModel() : this(string.Empty)
  {
  }

  public LocaleModel(string code) : this(CultureInfo.GetCultureInfo(code))
  {
  }

  public LocaleModel(CultureInfo culture)
  {
    LCID = culture.LCID;
    Code = culture.Name;
    DisplayName = culture.DisplayName;
    EnglishName = culture.EnglishName;
    NativeName = culture.NativeName;
  }

  public override string ToString() => $"{DisplayName} ({Code})";
}
