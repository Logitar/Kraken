using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Contents;

public record ContentSortOption : SortOption
{
  public new ContentSort Field
  {
    get => Enum.Parse<ContentSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ContentSortOption() : this(ContentSort.DisplayName, isDescending: false)
  {
  }

  public ContentSortOption(ContentSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
