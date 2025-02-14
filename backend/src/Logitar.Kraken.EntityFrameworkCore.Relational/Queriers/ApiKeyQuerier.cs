using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.ApiKeys;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class ApiKeyQuerier : IApiKeyQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ApiKeyEntity> _apiKeys;
  private readonly IApplicationContext _applicationContext;

  public ApiKeyQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _apiKeys = context.ApiKeys;
    _applicationContext = applicationContext;
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

  public Task<SearchResults<ApiKeyModel>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
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
