namespace Logitar.Kraken.Contracts.Fields;

public record FieldValueUpdate
{
  public Guid Id { get; set; }
  public string? Value { get; set; }

  public FieldValueUpdate()
  {
  }

  public FieldValueUpdate(Guid id, string? value)
  {
    Id = id;
    Value = value;
  }
}
