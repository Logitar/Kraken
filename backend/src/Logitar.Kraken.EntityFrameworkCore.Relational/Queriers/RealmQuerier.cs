using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class RealmQuerier : IRealmQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<RealmEntity> _realms;

  public RealmQuerier(IActorService actorService, KrakenContext context)
  {
    _actorService = actorService;
    _realms = context.Realms;
  }

  public async Task<RealmId?> FindIdAsync(Slug uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = Helper.Normalize(uniqueSlug);

    string? streamId = await _realms.AsNoTracking()
      .Where(x => x.UniqueSlugNormalized == uniqueSlugNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new RealmId(streamId);
  }

  public async Task<RealmModel> ReadAsync(Realm realm, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm.Id, cancellationToken) ?? throw new InvalidOperationException($"The realm entity 'StreamId={realm.Id}' could not be found.");
  }
  public async Task<RealmModel?> ReadAsync(RealmId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<RealmModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return realm == null ? null : await MapAsync(realm, cancellationToken);
  }
  public async Task<RealmModel?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = Helper.Normalize(uniqueSlug);

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);

    return realm == null ? null : await MapAsync(realm, cancellationToken);
  }

  public Task<SearchResults<RealmModel>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private async Task<RealmModel> MapAsync(RealmEntity realm, CancellationToken cancellationToken)
  {
    return (await MapAsync([realm], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<RealmModel>> MapAsync(IEnumerable<RealmEntity> realms, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = realms.SelectMany(role => role.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return realms.Select(mapper.ToRealm).ToArray();
  }
}
