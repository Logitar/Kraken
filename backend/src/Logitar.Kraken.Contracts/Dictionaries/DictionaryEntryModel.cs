namespace Logitar.Kraken.Contracts.Dictionaries;

public record DictionaryEntryModel
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;

  public DictionaryEntryModel()
  {
  }

  public DictionaryEntryModel(KeyValuePair<string, string> pair) : this(pair.Key, pair.Value)
  {
  }

  public DictionaryEntryModel(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
