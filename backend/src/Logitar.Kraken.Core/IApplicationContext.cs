using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Tokens;

namespace Logitar.Kraken.Core;

public interface IApplicationContext
{
  ActorId? ActorId { get; }

  string BaseUrl { get; }

  RealmModel? Realm { get; }
  RealmId? RealmId { get; }
  Secret Secret { get; }
  IUserSettings UserSettings { get; }
  IUniqueNameSettings UniqueNameSettings { get; }
}
