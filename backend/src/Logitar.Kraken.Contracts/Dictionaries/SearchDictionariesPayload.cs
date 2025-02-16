using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Dictionaries;

public record SearchDictionariesPayload : SearchPayload
{
  public bool? IsEmpty { get; set; }

  public new List<DictionarySortOption> Sort { get; set; } = [];
}
