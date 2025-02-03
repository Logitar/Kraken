using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Messages;

public readonly struct MessageId
{
  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public MessageId(RealmId? realmId, Guid entityId)
  {
    RealmId = realmId;
    EntityId = entityId;

    StreamId = IdHelper.Encode(realmId, entityId);
  }
  public MessageId(StreamId streamId)
  {
    Tuple<RealmId?, Guid> parsed = IdHelper.Decode(streamId);
    RealmId = parsed.Item1;
    EntityId = parsed.Item2;

    StreamId = streamId;
  }

  public static MessageId NewId(RealmId? realmId) => new(realmId, Guid.NewGuid());

  public static bool operator ==(MessageId left, MessageId right) => left.Equals(right);
  public static bool operator !=(MessageId left, MessageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is MessageId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
