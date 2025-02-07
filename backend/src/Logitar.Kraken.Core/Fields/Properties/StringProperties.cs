using FluentValidation;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Fields.Validators;

namespace Logitar.Kraken.Core.Fields.Properties;

public record StringProperties : FieldTypeProperties, IStringProperties
{
  [JsonIgnore]
  public override DataType DataType { get; } = DataType.String;

  public int? MinimumLength { get; }
  public int? MaximumLength { get; }
  public string? Pattern { get; }

  [JsonConstructor]
  public StringProperties(int? minimumLength = null, int? maximumLength = null, string? pattern = null)
  {
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    Pattern = pattern;
    new StringPropertiesValidator().ValidateAndThrow(this);
  }

  public StringProperties(IStringProperties @string)
  {
    MinimumLength = @string.MinimumLength;
    MaximumLength = @string.MaximumLength;
    Pattern = @string.Pattern;
    new StringPropertiesValidator().ValidateAndThrow(this);
  }
}
