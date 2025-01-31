using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Roles;

public record SearchRolesPayload : SearchPayload
{
  public new List<RoleSortOption> Sort { get; set; } = [];
}
