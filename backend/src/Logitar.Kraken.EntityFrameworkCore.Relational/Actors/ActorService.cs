using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Caching;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Actors;

internal class ActorService : IActorService // TODO(fpion): implement
{
  private readonly ICacheService _cacheService;

  public ActorService(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
