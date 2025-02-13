using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core;

public interface IApplicationContext
{
  ActorId? ActorId { get; }

  RealmModel? Realm { get; }
  RealmId? RealmId { get; }
  string BaseUrl { get; }
  string Secret { get; }
  IRoleSettings RoleSettings { get; }
  IUserSettings UserSettings { get; }
}
