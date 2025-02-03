using FluentValidation;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Fields.Validators;

namespace Logitar.Kraken.Core.Fields.Properties;

public record RichTextProperties : FieldTypeProperties, IRichTextProperties
{
  public override DataType DataType { get; } = DataType.RichText;

  public string ContentType { get; }
  public int? MinimumLength { get; }
  public int? MaximumLength { get; }

  [JsonConstructor]
  public RichTextProperties(string contentType, int? minimumLength = null, int? maximumLength = null)
  {
    ContentType = contentType;
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    new RichTextPropertiesValidator().ValidateAndThrow(this);
  }

  public RichTextProperties(IRichTextProperties richText)
  {
    ContentType = richText.ContentType;
    MinimumLength = richText.MinimumLength;
    MaximumLength = richText.MaximumLength;
    new RichTextPropertiesValidator().ValidateAndThrow(this);
  }
}
