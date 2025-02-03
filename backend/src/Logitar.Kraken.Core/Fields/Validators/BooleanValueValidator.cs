using FluentValidation.Results;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class BooleanValueValidator : IFieldValueValidator
{
  public Task<ValidationResult> ValidateAsync(string value, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 1);

    if (!bool.TryParse(value, out _))
    {
      ValidationFailure failure = new(propertyName, "The value is not a valid boolean.", value)
      {
        ErrorCode = nameof(BooleanValueValidator)
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
