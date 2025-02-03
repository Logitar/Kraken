using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Contents;

public record SearchContentTypesPayload : SearchPayload
{
  public bool? IsInvariant { get; set; }

  public new List<ContentTypeSortOption> Sort { get; set; } = [];
}
