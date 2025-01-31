using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Roles;

public record RoleSortOption : SortOption
{
  public new RoleSort Field
  {
    get => Enum.Parse<RoleSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public RoleSortOption() : this(RoleSort.DisplayName)
  {
  }

  public RoleSortOption(RoleSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
