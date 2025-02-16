using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.ApiKeys;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class ApiKeyQuerier : IApiKeyQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ApiKeyEntity> _apiKeys;
  private readonly IApplicationContext _applicationContext;
  private readonly IQueryHelper _queryHelper;

  public ApiKeyQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _apiKeys = context.ApiKeys;
    _applicationContext = applicationContext;
    _queryHelper = queryHelper;
  }

  public async Task<ApiKeyModel> ReadAsync(ApiKey apikey, CancellationToken cancellationToken)
  {
    return await ReadAsync(apikey.Id, cancellationToken) ?? throw new InvalidOperationException($"The API key entity 'StreamId={apikey.Id}' could not be found.");
  }
  public async Task<ApiKeyModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new ApiKeyId(id, _applicationContext.RealmId), cancellationToken);
  }
  public async Task<ApiKeyModel?> ReadAsync(ApiKeyId id, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await _apiKeys.AsNoTracking()
      .Include(x => x.Roles)
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return apiKey == null ? null : await MapAsync(apiKey, cancellationToken);
  }

  public async Task<SearchResults<ApiKeyModel>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(ApiKeys.Table).SelectAll(ApiKeys.Table)
      .WhereRealm(ApiKeys.RealmUid, _applicationContext.RealmId)
      .ApplyIdFilter(ApiKeys.Id, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, ApiKeys.DisplayName);

    if (payload.HasAuthenticated.HasValue)
    {
      NullOperator @operator = payload.HasAuthenticated.Value ? Operators.IsNotNull() : Operators.IsNull();
      builder.Where(ApiKeys.AuthenticatedOn, @operator);
    }
    if (payload.RoleId.HasValue)
    {
      builder.Join(ApiKeyRoles.ApiKeyId, ApiKeys.ApiKeyId)
        .Join(Roles.RoleId, ApiKeyRoles.RoleId, new OperatorCondition(Roles.Id, Operators.IsEqualTo(payload.RoleId.Value)));
    }
    if (payload.Status != null)
    {
      DateTime moment = payload.Status.Moment?.ToUniversalTime() ?? DateTime.UtcNow;
      builder.Where(ApiKeys.ExpiresOn, payload.Status.IsExpired ? Operators.IsLessThanOrEqualTo(moment) : Operators.IsGreaterThan(moment));
    }

    IQueryable<ApiKeyEntity> query = _apiKeys.FromQuery(builder).AsNoTracking()
      .Include(x => x.Roles);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ApiKeyEntity>? ordered = null;
    foreach (ApiKeySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ApiKeySort.AuthenticatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.AuthenticatedOn) : query.OrderBy(x => x.AuthenticatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.AuthenticatedOn) : ordered.ThenBy(x => x.AuthenticatedOn));
          break;
        case ApiKeySort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case ApiKeySort.ExpiresOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.ExpiresOn) : query.OrderBy(x => x.ExpiresOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.ExpiresOn) : ordered.ThenBy(x => x.ExpiresOn));
          break;
        case ApiKeySort.Name:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case ApiKeySort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ApiKeyEntity[] apiKeys = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ApiKeyModel> items = await MapAsync(apiKeys, cancellationToken);

    return new SearchResults<ApiKeyModel>(items, total);
  }

  private async Task<ApiKeyModel> MapAsync(ApiKeyEntity apiKey, CancellationToken cancellationToken)
  {
    return (await MapAsync([apiKey], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ApiKeyModel>> MapAsync(IEnumerable<ApiKeyEntity> apiKeys, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = apiKeys.SelectMany(apiKey => apiKey.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return apiKeys.Select(apiKey => mapper.ToApiKey(apiKey, _applicationContext.Realm)).ToArray();
  }
}
