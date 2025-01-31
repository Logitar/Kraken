namespace Logitar.Kraken.Core;

public record Error
{
  public string Code { get; set; } = string.Empty;
  public string Message { get; set; } = string.Empty;
  public Dictionary<string, object?> Data { get; set; } = [];

  public Error()
  {
  }

  public Error(string code, string message, IEnumerable<KeyValuePair<string, object?>>? data = null)
  {
    Code = code;
    Message = message;

    if (data != null)
    {
      foreach (KeyValuePair<string, object?> item in data)
      {
        Data[item.Key] = item.Value;
      }
    }
  }
}
