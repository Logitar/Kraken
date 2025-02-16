using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class RoleQuerier : IRoleQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly IQueryHelper _queryHelper;
  private readonly DbSet<RoleEntity> _roles;

  public RoleQuerier(IActorService actorService, IApplicationContext applicationContext, IQueryHelper queryHelper, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _queryHelper = queryHelper;
    _roles = context.Roles;
  }

  public async Task<RoleId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _roles.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new RoleId(new StreamId(streamId));
  }
  public async Task<IReadOnlyCollection<RoleId>> FindIdsAsync(IEnumerable<string> uniqueNames, CancellationToken cancellationToken)
  {
    HashSet<string> uniqueNameNormalized = uniqueNames.Select(Helper.Normalize).ToHashSet();

    string[] streamIds = await _roles.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => uniqueNameNormalized.Contains(x.UniqueNameNormalized))
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return streamIds.Select(streamId => new RoleId(new StreamId(streamId))).ToArray();
  }

  public async Task<RoleModel> ReadAsync(Role role, CancellationToken cancellationToken)
  {
    return await ReadAsync(role.Id, cancellationToken) ?? throw new InvalidOperationException($"The role entity 'StreamId={role.Id}' could not be found.");
  }
  public async Task<RoleModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new RoleId(id, _applicationContext.RealmId), cancellationToken);
  }
  public async Task<RoleModel?> ReadAsync(RoleId id, CancellationToken cancellationToken)
  {
    RoleEntity? role = await _roles.AsNoTracking()
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return role == null ? null : await MapAsync(role, cancellationToken);
  }
  public async Task<RoleModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    RoleEntity? role = await _roles.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return role == null ? null : await MapAsync(role, cancellationToken);
  }

  public async Task<SearchResults<RoleModel>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(Roles.Table).SelectAll(Roles.Table)
      .WhereRealm(Roles.RealmUid, _applicationContext.RealmId)
      .ApplyIdFilter(Roles.Id, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, Roles.UniqueName, Roles.DisplayName);

    IQueryable<RoleEntity> query = _roles.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<RoleEntity>? ordered = null;
    foreach (RoleSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case RoleSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case RoleSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case RoleSort.UniqueName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case RoleSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    RoleEntity[] roles = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<RoleModel> items = await MapAsync(roles, cancellationToken);

    return new SearchResults<RoleModel>(items, total);
  }

  private async Task<RoleModel> MapAsync(RoleEntity role, CancellationToken cancellationToken)
  {
    return (await MapAsync([role], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<RoleModel>> MapAsync(IEnumerable<RoleEntity> roles, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = roles.SelectMany(role => role.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return roles.Select(role => mapper.ToRole(role, _applicationContext.Realm)).ToArray();
  }
}
