namespace Logitar.Kraken.Contracts.Dictionaries;

public record UpdateDictionaryPayload
{
  public string? Language { get; set; }
  public List<DictionaryEntryModel> Entries { get; set; } = [];
}
