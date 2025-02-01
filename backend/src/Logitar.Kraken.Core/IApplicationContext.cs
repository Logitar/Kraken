using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;

namespace Logitar.Kraken.Core;

public interface IApplicationContext
{
  ActorId? ActorId { get; }
  RealmId? RealmId { get; }
  IRoleSettings RoleSettings { get; }
}
