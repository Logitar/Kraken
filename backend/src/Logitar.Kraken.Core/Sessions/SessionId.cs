using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Sessions;

public readonly struct SessionId
{
  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public SessionId(RealmId? realmId, Guid entityId)
  {
    RealmId = realmId;
    EntityId = entityId;

    StreamId = IdHelper.Encode(realmId, entityId);
  }
  public SessionId(StreamId streamId)
  {
    Tuple<RealmId?, Guid> parsed = IdHelper.Decode(streamId);
    RealmId = parsed.Item1;
    EntityId = parsed.Item2;

    StreamId = streamId;
  }

  public static SessionId NewId(RealmId? realmId) => new(realmId, Guid.NewGuid());

  public static bool operator ==(SessionId left, SessionId right) => left.Equals(right);
  public static bool operator !=(SessionId left, SessionId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is SessionId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
