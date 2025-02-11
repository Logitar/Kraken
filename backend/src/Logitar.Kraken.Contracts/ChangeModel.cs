namespace Logitar.Kraken.Contracts;

public record ChangeModel<T>
{
  public T? Value { get; set; }

  public ChangeModel(T? value = default)
  {
    Value = value;
  }
}
