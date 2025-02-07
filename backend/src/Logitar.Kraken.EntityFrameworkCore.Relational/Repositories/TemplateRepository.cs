using Logitar.EventSourcing;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Templates;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class TemplateRepository : Repository, ITemplateRepository
{
  private readonly DbSet<TemplateEntity> _templates;

  public TemplateRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _templates = context.Templates;
  }

  public async Task<Template?> LoadAsync(TemplateId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<Template?> LoadAsync(TemplateId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<Template?> LoadAsync(TemplateId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<Template?> LoadAsync(TemplateId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Template>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Template>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync(isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Template>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Template>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Template>> LoadAsync(IEnumerable<TemplateId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Template>> LoadAsync(IEnumerable<TemplateId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Template>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Template>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();

    string[] streamIds = await _templates.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<Template>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task<Template?> LoadAsync(RealmId? realmId, Identifier uniqueKey, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();
    string uniqueKeyNormalized = Helper.Normalize(uniqueKey);

    string? streamId = await _templates.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => (id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null) && x.UniqueKeyNormalized == uniqueKeyNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync<Template>(new StreamId(streamId), cancellationToken);
  }

  public async Task SaveAsync(Template template, CancellationToken cancellationToken)
  {
    await base.SaveAsync(template, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Template> templates, CancellationToken cancellationToken)
  {
    await base.SaveAsync(templates, cancellationToken);
  }
}
