using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Fields;

public readonly struct FieldTypeId
{
  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public FieldTypeId(RealmId? realmId, Guid entityId)
  {
    RealmId = realmId;
    EntityId = entityId;

    StreamId = IdHelper.Encode(realmId, entityId);
  }
  public FieldTypeId(StreamId streamId)
  {
    Tuple<RealmId?, Guid> parsed = IdHelper.Decode(streamId);
    RealmId = parsed.Item1;
    EntityId = parsed.Item2;

    StreamId = streamId;
  }

  public static FieldTypeId NewId(RealmId? realmId) => new(realmId, Guid.NewGuid());

  public static bool operator ==(FieldTypeId left, FieldTypeId right) => left.Equals(right);
  public static bool operator !=(FieldTypeId left, FieldTypeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is FieldTypeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
