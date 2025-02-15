using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Actors;

internal class ActorService : IActorService
{
  private readonly DbSet<ActorEntity> _actors;

  public ActorService(KrakenContext context)
  {
    _actors = context.Actors;
  }

  public async Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    if (!ids.Any())
    {
      return [];
    }

    IEnumerable<string> keys = ids.Select(id => id.Value).Distinct();
    ActorEntity[] actors = await _actors.AsNoTracking()
      .Where(actor => keys.Contains(actor.Key))
      .ToArrayAsync(cancellationToken);

    return actors.Select(Mapper.ToActor).ToArray();
  }
}
