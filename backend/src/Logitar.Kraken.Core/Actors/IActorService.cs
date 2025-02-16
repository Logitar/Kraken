using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;

namespace Logitar.Kraken.Core.Actors;

public interface IActorService
{
  Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}
