using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Fields;

public interface IFieldTypeRepository
{
  Task<FieldType?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken = default);
  Task<FieldType?> LoadAsync(FieldTypeId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<FieldType?> LoadAsync(FieldTypeId id, long? version, CancellationToken cancellationToken = default);
  Task<FieldType?> LoadAsync(FieldTypeId id, long? version, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<FieldType>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<FieldType>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<FieldType>> LoadAsync(IEnumerable<FieldTypeId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<FieldType>> LoadAsync(IEnumerable<FieldTypeId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<FieldType?> LoadAsync(RealmId? realmId, UniqueName uniqueName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<FieldType>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);

  Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<FieldType> fieldTypes, CancellationToken cancellationToken = default);
}
