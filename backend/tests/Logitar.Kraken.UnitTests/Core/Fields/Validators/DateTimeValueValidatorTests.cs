using Logitar.Kraken.Core.Fields.Properties;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Logitar.Kraken.Core.Fields.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class DateTimeValueValidatorTests
{
  private const string PropertyName = "PublicationDate";

  private readonly CancellationToken _cancellationToken = default;

  private readonly DateTimeProperties _properties = new(minimumValue: new DateTime(1950, 1, 1, 0, 0, 0), maximumValue: new DateTime(1999, 12, 31, 23, 59, 59));
  private readonly DateTimeValueValidator _validator;

  public DateTimeValueValidatorTests()
  {
    _validator = new(_properties);
  }

  [Fact(DisplayName = "Validation should fail when the value is not a valid DateTime.")]
  public async Task Given_NotDateTime_When_ValidateAsync_Then_FailureResult()
  {
    string value = "invalid";
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "DateTimeValueValidator" && e.ErrorMessage == "The value is not a valid DateTime."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName);
  }

  [Fact(DisplayName = "Validation should fail when the value is too high.")]
  public async Task Given_TooHigh_When_ValidateAsync_Then_FailureResult()
  {
    string value = DateTime.Now.ToISOString();
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "LessThanOrEqualValidator" && e.ErrorMessage == $"The value must be less than or equal to {_properties.MaximumValue}."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName && HasProperty(e.CustomState, "MaximumValue", _properties.MaximumValue));
  }

  [Fact(DisplayName = "Validation should fail when the value is too low.")]
  public async Task Given_TooLow_When_ValidateAsync_Then_FailureResult()
  {
    string value = new DateTime(1900, 1, 1, 0, 0, 0).ToISOString();
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.ErrorMessage == $"The value must be greater than or equal to {_properties.MinimumValue}."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName && HasProperty(e.CustomState, "MinimumValue", _properties.MinimumValue));
  }

  [Fact(DisplayName = "Validation should succeed when the value is valid.")]
  public async Task Given_ValidValue_When_ValidateAsync_Then_SuccessResult()
  {
    string value = new DateTime(1990, 8, 15, 12, 34, 56).ToISOString();
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.True(result.IsValid);
  }

  private static bool HasProperty(object instance, string propertyName, object? propertyValue)
  {
    PropertyInfo? property = instance.GetType().GetProperty(propertyName);
    Assert.NotNull(property);

    object? value = property.GetValue(instance);
    return propertyValue == null ? value == null : propertyValue.Equals(value);
  }
}
