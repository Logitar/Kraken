using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Contents;

public readonly struct ContentTypeId
{
  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public ContentTypeId(RealmId? realmId, Guid entityId)
  {
    RealmId = realmId;
    EntityId = entityId;

    StreamId = IdHelper.Encode(realmId, entityId);
  }
  public ContentTypeId(StreamId streamId)
  {
    Tuple<RealmId?, Guid> parsed = IdHelper.Decode(streamId);
    RealmId = parsed.Item1;
    EntityId = parsed.Item2;

    StreamId = streamId;
  }

  public static ContentTypeId NewId(RealmId? realmId) => new(realmId, Guid.NewGuid());

  public static bool operator ==(ContentTypeId left, ContentTypeId right) => left.Equals(right);
  public static bool operator !=(ContentTypeId left, ContentTypeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ContentTypeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
