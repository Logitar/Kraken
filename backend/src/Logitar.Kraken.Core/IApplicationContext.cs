using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;

namespace Logitar.Kraken.Core;

public interface IApplicationContext
{
  ActorId? ActorId { get; }

  public RealmModel? Realm { get; }
  RealmId? RealmId { get; }
  string BaseUrl { get; }
  string Secret { get; }
  IRoleSettings RoleSettings { get; }
  IUserSettings UserSettings { get; }
}
