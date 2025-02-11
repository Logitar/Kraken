using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Users;

public readonly struct UserId
{
  public RealmId? RealmId { get; }
  public Guid EntityId { get; }
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public UserId(Guid entityId, RealmId? realmId = null)
  {
    RealmId = realmId;
    EntityId = entityId;

    StreamId = IdHelper.Encode(realmId, entityId);
  }
  public UserId(StreamId streamId)
  {
    Tuple<RealmId?, Guid> decoded = IdHelper.Decode(streamId);
    RealmId = decoded.Item1;
    EntityId = decoded.Item2;

    StreamId = streamId;
  }

  public static UserId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public static bool operator ==(UserId left, UserId right) => left.Equals(right);
  public static bool operator !=(UserId left, UserId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is UserId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
