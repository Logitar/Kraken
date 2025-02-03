using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Templates;

public readonly struct TemplateId
{
  public RealmId? RealmId { get; }
  public Guid EntityId { get; }

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public TemplateId(RealmId? realmId, Guid entityId)
  {
    RealmId = realmId;
    EntityId = entityId;

    StreamId = IdHelper.Encode(realmId, entityId);
  }
  public TemplateId(StreamId streamId)
  {
    Tuple<RealmId?, Guid> parsed = IdHelper.Decode(streamId);
    RealmId = parsed.Item1;
    EntityId = parsed.Item2;

    StreamId = streamId;
  }

  public static TemplateId NewId(RealmId? realmId) => new(realmId, Guid.NewGuid());

  public static bool operator ==(TemplateId left, TemplateId right) => left.Equals(right);
  public static bool operator !=(TemplateId left, TemplateId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is TemplateId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
