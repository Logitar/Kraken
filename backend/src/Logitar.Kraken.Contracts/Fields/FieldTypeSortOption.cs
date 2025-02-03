using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Fields;

public record FieldTypeSortOption : SortOption
{
  public new FieldTypeSort Field
  {
    get => Enum.Parse<FieldTypeSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public FieldTypeSortOption() : this(FieldTypeSort.DisplayName, isDescending: false)
  {
  }

  public FieldTypeSortOption(FieldTypeSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
