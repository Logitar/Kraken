namespace Logitar.Kraken.Contracts.Messages;

public record Variable
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;

  public Variable()
  {
  }

  public Variable(KeyValuePair<string, string> variable) : this(variable.Key, variable.Value)
  {
  }

  public Variable(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
