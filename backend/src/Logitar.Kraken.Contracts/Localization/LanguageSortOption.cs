using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Localization;

public record LanguageSortOption : SortOption
{
  public new LanguageSort Field
  {
    get => Enum.Parse<LanguageSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public LanguageSortOption() : this(LanguageSort.DisplayName)
  {
  }

  public LanguageSortOption(LanguageSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
