namespace Logitar.Kraken.Contracts;

public record CustomIdentifierModel
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;

  public CustomIdentifierModel()
  {
  }

  public CustomIdentifierModel(KeyValuePair<string, string> customIdentifier) : this(customIdentifier.Key, customIdentifier.Value)
  {
  }

  public CustomIdentifierModel(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
