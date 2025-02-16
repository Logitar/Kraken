using Logitar.Data;
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
  private readonly IQueryHelper _queryHelper;
  private readonly DbSet<RealmEntity> _realms;

  public RealmQuerier(IActorService actorService, KrakenContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _queryHelper = queryHelper;
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

  public async Task<SearchResults<RealmModel>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(Realms.Table).SelectAll(Realms.Table)
      .ApplyIdFilter(Realms.Id, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, Realms.UniqueSlug, Realms.DisplayName);

    IQueryable<RealmEntity> query = _realms.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<RealmEntity>? ordered = null;
    foreach (RealmSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case RealmSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case RealmSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case RealmSort.UniqueSlug:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueSlug) : query.OrderBy(x => x.UniqueSlug))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueSlug) : ordered.ThenBy(x => x.UniqueSlug));
          break;
        case RealmSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    RealmEntity[] realms = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<RealmModel> items = await MapAsync(realms, cancellationToken);

    return new SearchResults<RealmModel>(items, total);
  }

  private async Task<RealmModel> MapAsync(RealmEntity realm, CancellationToken cancellationToken)
  {
    return (await MapAsync([realm], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<RealmModel>> MapAsync(IEnumerable<RealmEntity> realms, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = realms.SelectMany(realm => realm.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return realms.Select(mapper.ToRealm).ToArray();
  }
}
