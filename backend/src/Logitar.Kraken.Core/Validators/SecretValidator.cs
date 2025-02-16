using FluentValidation;
using FluentValidation.Validators;
using Logitar.Kraken.Core.Tokens;

namespace Logitar.Kraken.Core.Validators;

internal class SecretValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "SecretValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return $"'{{PropertyName}}' must be between {Secret.MinimumLength} and {Secret.MaximumLength} characters long (inclusive), excluding any spaces.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    value = value.Remove(" ");
    return value.Length >= Secret.MinimumLength && value.Length <= Secret.MaximumLength;
  }
}
