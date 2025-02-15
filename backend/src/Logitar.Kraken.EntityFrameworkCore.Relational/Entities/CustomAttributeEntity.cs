namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class CustomAttributeEntity
{
  public const int ValueShortenedLength = byte.MaxValue;

  public int CustomAttributeId { get; private set; }

  public string EntityType { get; private set; } = string.Empty;
  public int EntityId { get; private set; }

  public string Key { get; private set; } = string.Empty;
  public string Value { get; private set; } = string.Empty;
  public string ValueShortened
  {
    get => Value.Truncate(ValueShortenedLength);
    private set { }
  }

  public CustomAttributeEntity(string entityType, int entityId, string key, string value)
  {
    EntityType = entityType;
    EntityId = entityId;

    Key = key;

    Update(value);
  }

  private CustomAttributeEntity()
  {
  }

  public void Update(string value)
  {
    Value = value;
  }

  public override bool Equals(object? obj) => obj is CustomAttributeEntity entity && entity.CustomAttributeId == CustomAttributeId;
  public override int GetHashCode() => CustomAttributeId.GetHashCode();
  public override string ToString() => $"{GetType()} (CustomAttributeId={CustomAttributeId})";
}
