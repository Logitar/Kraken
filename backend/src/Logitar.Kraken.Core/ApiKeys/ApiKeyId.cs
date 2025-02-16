using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.ApiKeys;

public readonly struct ApiKeyId
{
  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public ApiKeyId(Guid entityId, RealmId? realmId = null)
  {
    RealmId = realmId;
    EntityId = entityId;

    StreamId = IdHelper.Encode(realmId, entityId);
  }
  public ApiKeyId(StreamId streamId)
  {
    Tuple<RealmId?, Guid> parsed = IdHelper.Decode(streamId);
    RealmId = parsed.Item1;
    EntityId = parsed.Item2;

    StreamId = streamId;
  }

  public static ApiKeyId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public static bool operator ==(ApiKeyId left, ApiKeyId right) => left.Equals(right);
  public static bool operator !=(ApiKeyId left, ApiKeyId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ApiKeyId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
