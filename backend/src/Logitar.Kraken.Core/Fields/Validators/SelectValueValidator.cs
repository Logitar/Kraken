using FluentValidation.Results;
using Logitar.Kraken.Core.Fields.Properties;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class SelectValueValidator : IFieldValueValidator
{
  private readonly ISelectProperties _properties;

  public SelectValueValidator(ISelectProperties properties)
  {
    _properties = properties;
  }

  public Task<ValidationResult> ValidateAsync(string inputValue, string propertyName, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<string> values;
    if (_properties.IsMultiple)
    {
      if (!TryParse(inputValue, out values))
      {
        ValidationFailure failure = new(propertyName, "The value must be a JSON-serialized string array.", inputValue)
        {
          ErrorCode = nameof(SelectValueValidator)
        };
        return Task.FromResult(new ValidationResult([failure]));
      }
    }
    else
    {
      values = [inputValue];
    }

    List<ValidationFailure> failures = new(capacity: values.Count);

    HashSet<string> allowedValues = _properties.Options.Select(option => option.Value ?? option.Text).ToHashSet();
    foreach (string value in values)
    {
      if (!allowedValues.Contains(value))
      {
        ValidationFailure failure = new(propertyName, $"The value should be one of the following: {string.Join(", ", allowedValues)}.", value)
        {
          CustomState = new { AllowedValues = allowedValues },
          ErrorCode = "OptionValidator"
        };
        failures.Add(failure);
      }
    }

    return Task.FromResult(new ValidationResult(failures));
  }

  private static bool TryParse(string value, out IReadOnlyCollection<string> values)
  {
    IReadOnlyCollection<string>? deserialized = null;
    try
    {
      deserialized = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(value);
    }
    catch (Exception)
    {
    }

    values = deserialized ?? [];
    return deserialized != null;
  }
}
