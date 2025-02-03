using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Contents;

public record ContentTypeSortOption : SortOption
{
  public new ContentTypeSort Field
  {
    get => Enum.Parse<ContentTypeSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ContentTypeSortOption() : this(ContentTypeSort.DisplayName, isDescending: false)
  {
  }

  public ContentTypeSortOption(ContentTypeSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
