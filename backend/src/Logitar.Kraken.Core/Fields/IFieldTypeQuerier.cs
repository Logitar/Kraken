using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Core.Fields;

public interface IFieldTypeQuerier
{
  Task<FieldTypeId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<FieldTypeModel> ReadAsync(FieldType fieldType, CancellationToken cancellationToken = default);
  Task<FieldTypeModel?> ReadAsync(FieldTypeId id, CancellationToken cancellationToken = default);
  Task<FieldTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<FieldTypeModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<FieldTypeModel>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken = default);
}
