namespace Logitar.Kraken.Contracts.Fields;

public record FieldValue
{
  public Guid Id { get; set; }
  public string Value { get; set; } = string.Empty;

  public FieldValue()
  {
  }

  public FieldValue(KeyValuePair<Guid, string> pair)
  {
    Id = pair.Key;
    Value = pair.Value;
  }

  public FieldValue(Guid id, string value)
  {
    Id = id;
    Value = value;
  }
}
