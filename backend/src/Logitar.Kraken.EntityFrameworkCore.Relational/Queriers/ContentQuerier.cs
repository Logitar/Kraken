using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class ContentQuerier : IContentQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<ContentEntity> _contents;
  private readonly DbSet<ContentLocaleEntity> _contentLocales;
  private readonly DbSet<UniqueIndexEntity> _uniqueIndex;

  public ContentQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _contents = context.Contents;
    _contentLocales = context.ContentLocales;
    _uniqueIndex = context.UniqueIndex;
  }

  public async Task<IReadOnlyDictionary<Guid, ContentId>> FindConflictsAsync(
    ContentTypeId contentTypeId,
    LanguageId? languageId,
    IReadOnlyDictionary<Guid, string> fieldValues,
    ContentId contentId,
    CancellationToken cancellationToken)
  {
    RealmId? realmId = _applicationContext.RealmId;
    Guid contentTypeUid = contentTypeId.EntityId;
    Guid? languageUid = languageId?.EntityId;
    HashSet<string> keys = fieldValues.Select(UniqueIndexEntity.CreateKey).ToHashSet();
    Guid contentUid = contentId.EntityId;

    var conflicts = await _uniqueIndex.AsNoTracking()
      .WhereRealm(realmId)
      .Where(x => x.ContentTypeUid == contentTypeUid
        && (languageUid.HasValue ? x.LanguageUid == languageUid.Value : x.LanguageUid == null)
        && keys.Contains(x.Key)
        && x.ContentUid != contentUid)
      .Select(x => new
      {
        FieldDefinitionId = x.FieldDefinitionUid,
        ContentId = x.ContentUid
      })
      .ToArrayAsync(cancellationToken);

    return conflicts.ToDictionary(x => x.FieldDefinitionId, x => new ContentId(realmId, x.ContentId));
  }

  public async Task<IReadOnlyDictionary<Guid, Guid>> FindContentTypeIdsAsync(IEnumerable<Guid> contentIds, CancellationToken cancellationToken)
  {
    HashSet<Guid> distinctIds = contentIds.ToHashSet();

    var associations = await _contents.AsNoTracking()
      .Include(x => x.ContentType)
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => distinctIds.Contains(x.Id))
      .Select(x => new
      {
        ContentId = x.Id,
        ContentTypeId = x.ContentType!.Id
      })
      .ToArrayAsync(cancellationToken);

    return associations.ToDictionary(x => x.ContentId, x => x.ContentTypeId);
  }

  public async Task<ContentId?> FindIdAsync(ContentTypeId contentTypeId, LanguageId? languageId, UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string contentTypeStreamId = contentTypeId.Value;
    string? languageStreamId = languageId?.Value;
    string uniqueNameNormalized = Helper.Normalize(uniqueName.Value);

    string? streamId = await _contentLocales.AsNoTracking()
      .Include(x => x.Content)
      .Include(x => x.ContentType)
      .Include(x => x.Language)
      .Where(x => x.ContentType!.StreamId == contentTypeStreamId
        && (languageStreamId == null ? x.Language!.StreamId == null : x.Language!.StreamId == languageStreamId)
        && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.Content!.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new ContentId(new StreamId(streamId));
  }

  public async Task<ContentModel> ReadAsync(Content content, CancellationToken cancellationToken)
  {
    return await ReadAsync(content.Id, cancellationToken) ?? throw new InvalidOperationException($"The content entity 'StreamId={content.Id}' could not be found.");
  }
  public async Task<ContentModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new ContentId(_applicationContext.RealmId, id), cancellationToken);
  }
  public async Task<ContentModel?> ReadAsync(ContentId id, CancellationToken cancellationToken)
  {
    ContentEntity? content = await _contents.AsNoTracking()
      .Include(x => x.ContentType).ThenInclude(x => x!.Fields).ThenInclude(x => x.FieldType)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return content == null ? null : await MapAsync(content, cancellationToken);
  }
  public async Task<ContentModel?> ReadAsync(ContentKey key, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(key.UniqueName);

    ContentEntity? content = await _contents.AsNoTracking()
      .Include(x => x.ContentType).ThenInclude(x => x!.Fields).ThenInclude(x => x.FieldType)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.ContentType!.Id == key.ContentTypeId)
      .Where(x => x.Locales.Any(l => (key.LanguageId.HasValue ? l.Language!.Id == key.LanguageId.Value : l.LanguageId == null) && l.UniqueNameNormalized == uniqueNameNormalized))
      .SingleOrDefaultAsync(cancellationToken);

    return content == null ? null : await MapAsync(content, cancellationToken);
  }

  public Task<SearchResults<ContentLocaleModel>> SearchAsync(SearchContentsPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private async Task<ContentModel> MapAsync(ContentEntity contentType, CancellationToken cancellationToken)
  {
    return (await MapAsync([contentType], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ContentModel>> MapAsync(IEnumerable<ContentEntity> contents, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = contents.SelectMany(content => content.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return contents.Select(content => mapper.ToContent(content, _applicationContext.Realm)).ToArray();
  }
}
