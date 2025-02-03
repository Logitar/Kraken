using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Properties;

public record SelectProperties : FieldTypeProperties, ISelectProperties
{
  public override DataType DataType { get; } = DataType.Select;

  public bool IsMultiple { get; }
  public IReadOnlyCollection<SelectOption> Options { get; }

  [JsonConstructor]
  public SelectProperties(bool isMultiple = false, IReadOnlyCollection<SelectOption>? options = null)
  {
    IsMultiple = isMultiple;
    Options = options ?? [];
  }

  public SelectProperties(ISelectProperties select)
  {
    IsMultiple = select.IsMultiple;
    Options = select.Options;
  }
}
