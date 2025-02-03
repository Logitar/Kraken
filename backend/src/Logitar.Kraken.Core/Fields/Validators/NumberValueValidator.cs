using FluentValidation.Results;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class NumberValueValidator : IFieldValueValidator
{
  private readonly INumberProperties _properties;

  public NumberValueValidator(INumberProperties properties)
  {
    _properties = properties;
  }

  public Task<ValidationResult> ValidateAsync(string value, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 2);

    if (double.TryParse(value, out double number))
    {
      if (number < _properties.MinimumValue)
      {
        ValidationFailure failure = new(propertyName, $"The value must be greater than or equal to {_properties.MinimumValue}.", value)
        {
          CustomState = new { _properties.MinimumValue },
          ErrorCode = "GreaterThanOrEqualValidator"
        };
        failures.Add(failure);
      }
      if (number > _properties.MaximumValue)
      {
        ValidationFailure failure = new(propertyName, $"The value must be less than or equal to {_properties.MaximumValue}.", value)
        {
          CustomState = new { _properties.MaximumValue },
          ErrorCode = "LessThanOrEqualValidator"
        };
        failures.Add(failure);
      }
    }
    else
    {
      ValidationFailure failure = new(propertyName, "The value is not a valid number.", value)
      {
        ErrorCode = nameof(NumberValueValidator)
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
