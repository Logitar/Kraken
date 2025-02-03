using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;

namespace Logitar.Kraken.Core.Fields.Properties;

public record RelatedContentProperties : FieldTypeProperties, IRelatedContentProperties
{
  public override DataType DataType { get; } = DataType.RelatedContent;

  public ContentTypeId ContentTypeId { get; }
  public bool IsMultiple { get; }

  [JsonConstructor]
  public RelatedContentProperties(ContentTypeId contentTypeId, bool isMultiple = false)
  {
    ContentTypeId = contentTypeId;
    IsMultiple = isMultiple;
  }

  public RelatedContentProperties(IRelatedContentProperties relatedContent)
  {
    ContentTypeId = relatedContent.ContentTypeId;
    IsMultiple = relatedContent.IsMultiple;
  }
}
