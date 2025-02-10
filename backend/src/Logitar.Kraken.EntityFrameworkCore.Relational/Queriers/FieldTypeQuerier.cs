using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class FieldTypeQuerier : IFieldTypeQuerier
{
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<FieldTypeEntity> _fieldTypes;

  public FieldTypeQuerier(IApplicationContext applicationContext, KrakenContext context)
  {
    _applicationContext = applicationContext;
    _fieldTypes = context.FieldTypes;
  }

  public Task<FieldTypeId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
  }

  public async Task<FieldTypeModel> ReadAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    return await ReadAsync(fieldType.Id, cancellationToken) ?? throw new InvalidOperationException($"The field type entity 'StreamId={fieldType.Id}' could not be found.");
  }
  public async Task<FieldTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new FieldTypeId(_applicationContext.RealmId, id), cancellationToken);
  }
  public Task<FieldTypeModel?> ReadAsync(FieldTypeId id, CancellationToken cancellationToken)
  {
  }
  public Task<FieldTypeModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
  }

  public Task<SearchResults<FieldTypeModel>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken)
  {
  }
}
