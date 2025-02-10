using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Core.Actors;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Actors;

internal class ActorService : IActorService // ISSUE #89: https://github.com/Logitar/Kraken/issues/89
{
  public Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
