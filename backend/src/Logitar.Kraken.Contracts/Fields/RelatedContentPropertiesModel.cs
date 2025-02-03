namespace Logitar.Kraken.Contracts.Fields;

public record RelatedContentPropertiesModel
{
  public Guid ContentTypeId { get; set; }
  public bool IsMultiple { get; set; }
}
