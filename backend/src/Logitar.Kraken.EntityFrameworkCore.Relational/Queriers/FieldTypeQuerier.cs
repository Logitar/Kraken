using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class FieldTypeQuerier : IFieldTypeQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<FieldTypeEntity> _fieldTypes;

  public FieldTypeQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _fieldTypes = context.FieldTypes;
  }

  public async Task<FieldTypeId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _fieldTypes.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new FieldTypeId(new StreamId(streamId));
  }

  public async Task<FieldTypeModel> ReadAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    return await ReadAsync(fieldType.Id, cancellationToken) ?? throw new InvalidOperationException($"The field type entity 'StreamId={fieldType.Id}' could not be found.");
  }
  public async Task<FieldTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new FieldTypeId(_applicationContext.RealmId, id), cancellationToken);
  }
  public async Task<FieldTypeModel?> ReadAsync(FieldTypeId id, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _fieldTypes.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return fieldType == null ? null : await MapAsync(fieldType, cancellationToken);
  }
  public async Task<FieldTypeModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    FieldTypeEntity? fieldType = await _fieldTypes.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return fieldType == null ? null : await MapAsync(fieldType, cancellationToken);
  }

  public Task<SearchResults<FieldTypeModel>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private async Task<FieldTypeModel> MapAsync(FieldTypeEntity fieldType, CancellationToken cancellationToken)
  {
    return (await MapAsync([fieldType], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<FieldTypeModel>> MapAsync(IEnumerable<FieldTypeEntity> fieldTypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = fieldTypes.SelectMany(fieldType => fieldType.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return fieldTypes.Select(fieldType => mapper.ToFieldType(fieldType, _applicationContext.Realm)).ToArray();
  }
}
