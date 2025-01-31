using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Realms;

public record SearchRealmsPayload : SearchPayload
{
  public new List<RealmSortOption> Sort { get; set; } = [];
}
