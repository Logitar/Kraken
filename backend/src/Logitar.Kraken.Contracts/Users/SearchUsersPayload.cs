using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Users;

public record SearchUsersPayload : SearchPayload
{
  public bool? HasAuthenticated { get; set; }
  public bool? HasPassword { get; set; }
  public bool? IsDisabled { get; set; }
  public bool? IsConfirmed { get; set; }
  public Guid? RoleId { get; set; }

  public new List<UserSortOption> Sort { get; set; } = [];
}
