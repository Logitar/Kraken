using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields.Properties;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class FieldValueValidatorFactory : IFieldValueValidatorFactory
{
  private readonly IContentQuerier _contentQuerier;

  public FieldValueValidatorFactory(IContentQuerier contentQuerier)
  {
    _contentQuerier = contentQuerier;
  }

  public IFieldValueValidator Create(FieldType fieldType) => fieldType.DataType switch
  {
    DataType.Boolean => new BooleanValueValidator(),
    DataType.DateTime => new DateTimeValueValidator((DateTimeProperties)fieldType.Properties),
    DataType.Number => new NumberValueValidator((NumberProperties)fieldType.Properties),
    DataType.RelatedContent => new RelatedContentValueValidator(_contentQuerier, (RelatedContentProperties)fieldType.Properties),
    DataType.RichText => new RichTextValueValidator((RichTextProperties)fieldType.Properties),
    DataType.Select => new SelectValueValidator((SelectProperties)fieldType.Properties),
    DataType.String => new StringValueValidator((StringProperties)fieldType.Properties),
    DataType.Tags => new TagsValueValidator(),
    _ => throw new DataTypeNotSupportedException(fieldType.DataType),
  };
}
