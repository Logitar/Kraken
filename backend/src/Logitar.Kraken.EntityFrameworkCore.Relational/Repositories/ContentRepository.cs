using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class ContentRepository : Repository, IContentRepository
{
  private readonly DbSet<ContentEntity> _contents;

  public ContentRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _contents = context.Contents;
  }

  public async Task<Content?> LoadAsync(ContentId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<Content?> LoadAsync(ContentId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<Content?> LoadAsync(ContentId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<Content?> LoadAsync(ContentId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Content>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Content>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync(isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Content>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Content>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Content>> LoadAsync(IEnumerable<ContentId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Content>> LoadAsync(IEnumerable<ContentId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Content>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Content>> LoadAsync(ContentTypeId contentTypeId, CancellationToken cancellationToken)
  {
    Guid? realmId = contentTypeId.RealmId?.ToGuid();
    Guid id = contentTypeId.EntityId;

    string[] streamIds = await _contents.AsNoTracking()
      .Include(x => x.ContentType)
      .Include(x => x.Realm)
      .Where(x => (realmId.HasValue ? x.Realm!.Id == realmId.Value : x.RealmId == null) && x.ContentType!.Id == id)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<Content>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task<IReadOnlyCollection<Content>> LoadAsync(LanguageId languageId, CancellationToken cancellationToken)
  {
    Guid? realmId = languageId.RealmId?.ToGuid();
    Guid id = languageId.EntityId;

    string[] streamIds = await _contents.AsNoTracking()
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .Include(x => x.Realm)
      .Where(x => (realmId.HasValue ? x.Realm!.Id == realmId.Value : x.RealmId == null) && x.Locales.Any(l => l.Language!.Id == id))
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<Content>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task<IReadOnlyCollection<Content>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();

    string[] streamIds = await _contents.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<Content>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task SaveAsync(Content content, CancellationToken cancellationToken)
  {
    await base.SaveAsync(content, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Content> contents, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contents, cancellationToken);
  }
}
