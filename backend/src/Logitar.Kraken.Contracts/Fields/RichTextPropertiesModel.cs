namespace Logitar.Kraken.Contracts.Fields;

public record RichTextPropertiesModel : IRichTextProperties
{
  public string ContentType { get; set; } = string.Empty;
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }

  public RichTextPropertiesModel()
  {
  }

  public RichTextPropertiesModel(IRichTextProperties richText)
  {
    ContentType = richText.ContentType;
    MinimumLength = richText.MinimumLength;
    MaximumLength = richText.MaximumLength;
  }
}
