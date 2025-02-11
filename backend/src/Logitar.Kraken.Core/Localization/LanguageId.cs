using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Localization;

public readonly struct LanguageId
{
  public RealmId? RealmId { get; }
  public Guid EntityId { get; }
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public LanguageId(Guid entityId, RealmId? realmId = null)
  {
    RealmId = realmId;
    EntityId = entityId;

    StreamId = IdHelper.Encode(realmId, entityId);
  }
  public LanguageId(StreamId streamId)
  {
    Tuple<RealmId?, Guid> decoded = IdHelper.Decode(streamId);
    RealmId = decoded.Item1;
    EntityId = decoded.Item2;

    StreamId = streamId;
  }

  public static LanguageId NewId(RealmId? realmId = null) => new(Guid.NewGuid(), realmId);

  public static bool operator ==(LanguageId left, LanguageId right) => left.Equals(right);
  public static bool operator !=(LanguageId left, LanguageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is LanguageId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
