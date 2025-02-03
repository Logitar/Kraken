using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Contracts.Realms;

namespace Logitar.Kraken.Contracts.Dictionaries;

public class DictionaryModel : AggregateModel
{
  public LanguageModel Language { get; set; } = new();

  public int EntryCount { get; set; }
  public List<DictionaryEntryModel> Entries { get; set; } = [];

  public RealmModel? Realm { get; set; }
}
