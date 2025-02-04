namespace Logitar.Kraken.Contracts.Messages;

public record ResultData
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;

  public ResultData()
  {
  }

  public ResultData(KeyValuePair<string, string> data) : this(data.Key, data.Value)
  {
  }

  public ResultData(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
