using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Localization;

public record SearchLanguagesPayload : SearchPayload
{
  public new List<LanguageSortOption> Sort { get; set; } = [];
}
