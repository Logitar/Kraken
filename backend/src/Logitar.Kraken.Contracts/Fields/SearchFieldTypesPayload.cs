using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Fields;

public record SearchFieldTypesPayload : SearchPayload
{
  public DataType? DataType { get; set; }

  public new List<FieldTypeSortOption> Sort { get; set; } = [];
}
