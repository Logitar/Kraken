namespace Logitar.Kraken.Contracts;

public record CustomAttributeModel
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;

  public CustomAttributeModel()
  {
  }

  public CustomAttributeModel(KeyValuePair<string, string> customAttribute) : this(customAttribute.Key, customAttribute.Value)
  {
  }

  public CustomAttributeModel(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
