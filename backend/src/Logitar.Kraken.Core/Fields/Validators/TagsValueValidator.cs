using FluentValidation.Results;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class TagsValueValidator : IFieldValueValidator
{
  public Task<ValidationResult> ValidateAsync(string value, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 1);

    IReadOnlyCollection<string>? tags = TryParse(value);
    if (tags == null)
    {
      ValidationFailure failure = new(propertyName, "The value must be a JSON-serialized string array.", value)
      {
        ErrorCode = nameof(TagsValueValidator)
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }

  private static IReadOnlyCollection<string>? TryParse(string value)
  {
    IReadOnlyCollection<string>? values = null;
    try
    {
      values = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(value);
    }
    catch (Exception)
    {
    }
    return values;
  }
}
