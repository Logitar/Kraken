using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core;

public static class IdHelper
{
  private const char Separator = ':';

  public static Tuple<RealmId?, Guid> Decode(StreamId streamId)
  {
    RealmId? realmId = null;
    Guid entityId;

    int index = streamId.Value.IndexOf(Separator);
    if (index < 0)
    {
      entityId = streamId.ToGuid();
    }
    else
    {
      realmId = new RealmId(streamId.Value[..index]);
      entityId = new Guid(Convert.FromBase64String(streamId.Value[(index + 1)..].FromUriSafeBase64()));
    }

    return new Tuple<RealmId?, Guid>(realmId, entityId);
  }

  public static StreamId Encode(RealmId? realmId, Guid entityId)
  {
    string entityIdValue = Convert.ToBase64String(entityId.ToByteArray()).ToUriSafeBase64();
    return new StreamId(realmId.HasValue ? string.Join(Separator, realmId.Value, entityIdValue) : entityIdValue);
  }
}
