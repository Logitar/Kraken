using FluentValidation;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Fields.Validators;

namespace Logitar.Kraken.Core.Fields.Properties;

public record BooleanProperties : FieldTypeProperties, IBooleanProperties
{
  [JsonIgnore]
  public override DataType DataType { get; } = DataType.Boolean;

  [JsonConstructor]
  public BooleanProperties()
  {
    new BooleanPropertiesValidator().ValidateAndThrow(this);
  }

  public BooleanProperties(IBooleanProperties _)
  {
    new BooleanPropertiesValidator().ValidateAndThrow(this);
  }
}
