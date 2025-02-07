using FluentValidation;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Fields.Validators;

namespace Logitar.Kraken.Core.Fields.Properties;

public record NumberProperties : FieldTypeProperties, INumberProperties
{
  [JsonIgnore]
  public override DataType DataType { get; } = DataType.Number;

  public double? MinimumValue { get; }
  public double? MaximumValue { get; }
  public double? Step { get; }

  [JsonConstructor]
  public NumberProperties(double? minimumValue = null, double? maximumValue = null, double? step = null)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
    Step = step;
    new NumberPropertiesValidator().ValidateAndThrow(this);
  }

  public NumberProperties(INumberProperties number)
  {
    MinimumValue = number.MinimumValue;
    MaximumValue = number.MaximumValue;
    Step = number.Step;
    new NumberPropertiesValidator().ValidateAndThrow(this);
  }
}
