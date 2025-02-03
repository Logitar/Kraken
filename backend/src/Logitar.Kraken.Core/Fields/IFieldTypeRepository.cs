namespace Logitar.Kraken.Core.Fields;

public interface IFieldTypeRepository
{
  Task<FieldType?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken = default);
  Task<FieldType?> LoadAsync(FieldTypeId id, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<FieldType>> LoadAsync(IEnumerable<FieldTypeId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<FieldType> fieldTypes, CancellationToken cancellationToken = default);
}
