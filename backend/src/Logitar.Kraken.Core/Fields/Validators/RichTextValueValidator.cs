using FluentValidation.Results;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class RichTextValueValidator : IFieldValueValidator
{
  private readonly IRichTextProperties _properties;

  public RichTextValueValidator(IRichTextProperties properties)
  {
    _properties = properties;
  }

  public Task<ValidationResult> ValidateAsync(string value, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 2);

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

    return Task.FromResult(new ValidationResult(failures));
  }
}
