using FluentValidation;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Fields.Validators;

namespace Logitar.Kraken.Core.Fields.Properties;

public record TagsProperties : FieldTypeProperties, ITagsProperties
{
  [JsonIgnore]
  public override DataType DataType { get; } = DataType.Tags;

  [JsonConstructor]
  public TagsProperties()
  {
    new TagsPropertiesValidator().ValidateAndThrow(this);
  }

  public TagsProperties(ITagsProperties _)
  {
    new TagsPropertiesValidator().ValidateAndThrow(this);
  }
}
