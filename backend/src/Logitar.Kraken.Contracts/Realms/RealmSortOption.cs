using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Realms;

public record RealmSortOption : SortOption
{
  public new RealmSort Field
  {
    get => Enum.Parse<RealmSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public RealmSortOption() : this(RealmSort.DisplayName)
  {
  }

  public RealmSortOption(RealmSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
