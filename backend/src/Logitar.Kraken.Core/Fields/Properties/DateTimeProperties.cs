using FluentValidation;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Fields.Validators;

namespace Logitar.Kraken.Core.Fields.Properties;

public record DateTimeProperties : FieldTypeProperties, IDateTimeProperties
{
  [JsonIgnore]
  public override DataType DataType { get; } = DataType.DateTime;

  public DateTime? MinimumValue { get; }
  public DateTime? MaximumValue { get; }

  [JsonConstructor]
  public DateTimeProperties(DateTime? minimumValue = null, DateTime? maximumValue = null)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
    new DateTimePropertiesValidator().ValidateAndThrow(this);
  }

  public DateTimeProperties(IDateTimeProperties dateTime)
  {
    MinimumValue = dateTime.MinimumValue;
    MaximumValue = dateTime.MaximumValue;
    new DateTimePropertiesValidator().ValidateAndThrow(this);
  }
}
