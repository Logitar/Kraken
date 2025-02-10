using Logitar.EventSourcing;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class FieldTypeRepository : Repository, IFieldTypeRepository
{
  private readonly DbSet<FieldTypeEntity> _fieldTypes;

  public FieldTypeRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _fieldTypes = context.FieldTypes;
  }

  public async Task<FieldType?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<FieldType?> LoadAsync(FieldTypeId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<FieldType?> LoadAsync(FieldTypeId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<FieldType?> LoadAsync(FieldTypeId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<FieldType>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<FieldType>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync(isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<FieldType>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<FieldType>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<FieldType>> LoadAsync(IEnumerable<FieldTypeId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<FieldType>> LoadAsync(IEnumerable<FieldTypeId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<FieldType>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<FieldType?> LoadAsync(RealmId? realmId, UniqueName uniqueName, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _fieldTypes.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => (id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null) && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync<FieldType>(new StreamId(streamId), cancellationToken);
  }

  public async Task<IReadOnlyCollection<FieldType>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();

    string[] streamIds = await _fieldTypes.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<FieldType>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    await base.SaveAsync(fieldType, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<FieldType> fieldTypes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(fieldTypes, cancellationToken);
  }
}
