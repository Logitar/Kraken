namespace Logitar.Kraken.Contracts.Fields;

public record CreateOrReplaceFieldTypePayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public BooleanPropertiesModel? Boolean { get; set; }
  public DateTimePropertiesModel? DateTime { get; set; }
  public NumberPropertiesModel? Number { get; set; }
  public RelatedContentPropertiesModel? RelatedContent { get; set; }
  public RichTextPropertiesModel? RichText { get; set; }
  public SelectPropertiesModel? Select { get; set; }
  public StringPropertiesModel? String { get; set; }
  public TagsPropertiesModel? Tags { get; set; }
}
