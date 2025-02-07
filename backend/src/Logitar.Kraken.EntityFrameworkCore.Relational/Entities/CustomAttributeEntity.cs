namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class CustomAttributeEntity
{
  public const int ValueShortenedLength = byte.MaxValue;

  public int CustomAttributeId { get; set; }

  public string EntityType { get; set; } = string.Empty;
  public int EntityId { get; set; }

  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public string ValueShortened
  {
    get => Value.Truncate(ValueShortenedLength);
    private set { }
  }

  public override bool Equals(object? obj) => obj is CustomAttributeEntity entity && entity.CustomAttributeId == CustomAttributeId;
  public override int GetHashCode() => CustomAttributeId.GetHashCode();
  public override string ToString() => $"{GetType()} (CustomAttributeId={CustomAttributeId})";
}
