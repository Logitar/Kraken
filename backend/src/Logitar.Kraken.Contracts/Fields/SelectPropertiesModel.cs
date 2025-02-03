namespace Logitar.Kraken.Contracts.Fields;

public record SelectPropertiesModel
{
  public bool IsMultiple { get; set; }
  public List<SelectOptionModel> Options { get; set; } = [];
}
