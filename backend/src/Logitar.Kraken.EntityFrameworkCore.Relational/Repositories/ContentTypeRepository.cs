using Logitar.EventSourcing;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class ContentTypeRepository : Repository, IContentTypeRepository
{
  private readonly DbSet<ContentTypeEntity> _contentTypes;

  public ContentTypeRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _contentTypes = context.ContentTypes;
  }

  public async Task<ContentType?> LoadAsync(ContentTypeId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<ContentType?> LoadAsync(ContentTypeId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<ContentType?> LoadAsync(ContentTypeId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<ContentType?> LoadAsync(ContentTypeId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<ContentType>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<ContentType>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync(isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<ContentType>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ContentType>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<ContentType>> LoadAsync(IEnumerable<ContentTypeId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<ContentType>> LoadAsync(IEnumerable<ContentTypeId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ContentType>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<ContentType?> LoadAsync(RealmId? realmId, Identifier identifier, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();
    string uniqueNameNormalized = Helper.Normalize(identifier);

    string? streamId = await _contentTypes.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => (id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null) && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync<ContentType>(new StreamId(streamId), cancellationToken);
  }

  public async Task<IReadOnlyCollection<ContentType>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();

    string[] streamIds = await _contentTypes.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<ContentType>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task<ContentType> LoadAsync(Content content, CancellationToken cancellationToken)
  {
    return await LoadAsync(content.ContentTypeId, cancellationToken) ?? throw new InvalidOperationException($"The content type 'Id={content.ContentTypeId}' could not be found.");
  }

  public async Task<IReadOnlyCollection<ContentType>> LoadAsync(FieldTypeId fieldTypeId, CancellationToken cancellationToken)
  {
    Guid? realmId = fieldTypeId.RealmId?.ToGuid();
    Guid id = fieldTypeId.EntityId;

    string[] streamIds = await _contentTypes.AsNoTracking()
      .Include(x => x.Fields).ThenInclude(x => x.FieldType).ThenInclude(x => x!.Realm)
      .Where(x => x.Fields.Any(f => (realmId.HasValue ? f.FieldType!.Realm!.Id == realmId.Value : f.FieldType!.RealmId == null) && f.FieldType.Id == id))
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<ContentType>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task SaveAsync(ContentType contentType, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contentType, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<ContentType> contentTypes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contentTypes, cancellationToken);
  }
}
