using Logitar.EventSourcing;
using Logitar.Kraken.Core.Settings;

namespace Logitar.Kraken.Core;

public interface IApplicationContext
{
  ActorId? ActorId { get; }
  IRoleSettings RoleSettings { get; }
}
