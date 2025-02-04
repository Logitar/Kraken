using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Senders;

public record SearchSendersPayload : SearchPayload
{
  public SenderProvider? Provider { get; set; }

  public new List<SenderSortOption> Sort { get; set; } = [];
}
