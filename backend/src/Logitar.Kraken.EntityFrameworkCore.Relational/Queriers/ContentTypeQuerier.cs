using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class ContentTypeQuerier : IContentTypeQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<ContentTypeEntity> _contentTypes;

  public ContentTypeQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _contentTypes = context.ContentTypes;
  }

  public async Task<ContentTypeId?> FindIdAsync(Identifier uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _contentTypes.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new ContentTypeId(new StreamId(streamId));
  }

  public async Task<ContentTypeModel> ReadAsync(ContentType contentType, CancellationToken cancellationToken)
  {
    return await ReadAsync(contentType.Id, cancellationToken) ?? throw new InvalidOperationException($"The content type entity 'StreamId={contentType.Id}' could not be found.");
  }
  public async Task<ContentTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new ContentTypeId(_applicationContext.RealmId, id), cancellationToken);
  }
  public async Task<ContentTypeModel?> ReadAsync(ContentTypeId id, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await _contentTypes.AsNoTracking()
      .Include(x => x.Fields).ThenInclude(x => x.FieldType)
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return contentType == null ? null : await MapAsync(contentType, cancellationToken);
  }
  public async Task<ContentTypeModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    ContentTypeEntity? contentType = await _contentTypes.AsNoTracking()
      .Include(x => x.Fields).ThenInclude(x => x.FieldType)
      .WhereRealm(_applicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return contentType == null ? null : await MapAsync(contentType, cancellationToken);
  }

  public Task<SearchResults<ContentTypeModel>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private async Task<ContentTypeModel> MapAsync(ContentTypeEntity contentType, CancellationToken cancellationToken)
  {
    return (await MapAsync([contentType], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ContentTypeModel>> MapAsync(IEnumerable<ContentTypeEntity> contentTypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contentTypes.SelectMany(contentType => contentType.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return contentTypes.Select(contentType => mapper.ToContentType(contentType, _applicationContext.Realm)).ToArray();
  }
}
