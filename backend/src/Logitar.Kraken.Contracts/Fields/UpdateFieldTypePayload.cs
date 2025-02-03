namespace Logitar.Kraken.Contracts.Fields;

public record UpdateFieldTypePayload
{
  public string? UniqueName { get; set; } = string.Empty;
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }

  public BooleanPropertiesModel? Boolean { get; set; }
  public DateTimePropertiesModel? DateTime { get; set; }
  public NumberPropertiesModel? Number { get; set; }
  public RelatedContentPropertiesModel? RelatedContent { get; set; }
  public RichTextPropertiesModel? RichText { get; set; }
  public SelectPropertiesModel? Select { get; set; }
  public StringPropertiesModel? String { get; set; }
  public TagsPropertiesModel? Tags { get; set; }
}
