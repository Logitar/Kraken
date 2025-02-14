using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core;

public interface IApplicationContext
{
  ActorId? ActorId { get; }

  string BaseUrl { get; }

  RealmModel? Realm { get; }
  RealmId? RealmId { get; }
  string Secret { get; }
  IUserSettings UserSettings { get; }
  IUniqueNameSettings UniqueNameSettings { get; }
}
