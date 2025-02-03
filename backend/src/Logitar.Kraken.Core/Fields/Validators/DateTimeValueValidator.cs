using FluentValidation.Results;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class DateTimeValueValidator : IFieldValueValidator
{
  private readonly IDateTimeProperties _properties;

  public DateTimeValueValidator(IDateTimeProperties properties)
  {
    _properties = properties;
  }

  public Task<ValidationResult> ValidateAsync(string value, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 2);

    if (DateTime.TryParse(value, out DateTime dateTime))
    {
      if (dateTime < _properties.MinimumValue)
      {
        ValidationFailure failure = new(propertyName, $"The value must be greater than or equal to {_properties.MinimumValue}.", value)
        {
          CustomState = new { _properties.MinimumValue },
          ErrorCode = "GreaterThanOrEqualValidator"
        };
        failures.Add(failure);
      }
      if (dateTime > _properties.MaximumValue)
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
      ValidationFailure failure = new(propertyName, "The value is not a valid DateTime.", value)
      {
        ErrorCode = nameof(DateTimeValueValidator)
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
