using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Realms;

public record RealmSortOption : SortOption
{
  public new RealmSort Field
  {
    get => Enum.Parse<RealmSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public RealmSortOption(RealmSort field = RealmSort.UpdatedOn, bool isDescending = true)
  {
    Field = field;
    IsDescending = isDescending;
  }
}
