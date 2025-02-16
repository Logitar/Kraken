using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.ApiKeys;

public record SearchApiKeysPayload : SearchPayload
{
  public bool? HasAuthenticated { get; set; }
  public Guid? RoleId { get; set; }
  public ApiKeyStatus? Status { get; set; }

  public new List<ApiKeySortOption> Sort { get; set; } = [];
}
