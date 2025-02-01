using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.ApiKeys;

public record SearchApiKeysPayload : SearchPayload
{
  public Guid? RoleId { get; set; }
  public ApiKeyStatus? Status { get; set; }

  public new List<ApiKeySortOption> Sort { get; set; } = [];
}
