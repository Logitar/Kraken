using FluentValidation.Results;
using Logitar.Kraken.Contracts.Fields;
using System.Text.RegularExpressions;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class StringValueValidator : IFieldValueValidator
{
  private readonly IStringProperties _properties;

  public StringValueValidator(IStringProperties properties)
  {
    _properties = properties;
  }

  public Task<ValidationResult> ValidateAsync(string value, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 3);

    if (value.Length < _properties.MinimumLength)
    {
      ValidationFailure failure = new(propertyName, $"The length of the value must be at least {_properties.MinimumLength} characters.", value)
      {
        CustomState = new { _properties.MinimumLength },
        ErrorCode = "MinimumLengthValidator"
      };
      failures.Add(failure);
    }
    if (value.Length > _properties.MaximumLength)
    {
      ValidationFailure failure = new(propertyName, $"The length of the value may not exceed {_properties.MaximumLength} characters.", value)
      {
        CustomState = new { _properties.MaximumLength },
        ErrorCode = "MaximumLengthValidator"
      };
      failures.Add(failure);
    }
    if (_properties.Pattern != null && !Regex.IsMatch(value, _properties.Pattern))
    {
      ValidationFailure failure = new(propertyName, $"The value must match the pattern '{_properties.Pattern}'.", value)
      {
        CustomState = new { _properties.Pattern },
        ErrorCode = "RegularExpressionValidator"
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
