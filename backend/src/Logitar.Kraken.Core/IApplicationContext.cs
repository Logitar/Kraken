using Logitar.EventSourcing;

namespace Logitar.Kraken.Core;

public interface IApplicationContext
{
  ActorId? GetActorId();
}
