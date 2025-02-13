namespace Logitar.Kraken.Contracts.Dictionaries;

public record CreateOrReplaceDictionaryPayload
{
  public string Language { get; set; } = string.Empty;
  public List<DictionaryEntryModel> Entries { get; set; } = [];
}
