using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Localization;

public record LanguageSortOption : SortOption
{
  public new LanguageSort Field
  {
    get => Enum.Parse<LanguageSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public LanguageSortOption(LanguageSort field = LanguageSort.UpdatedOn, bool isDescending = true)
  {
    Field = field;
    IsDescending = isDescending;
  }
}
