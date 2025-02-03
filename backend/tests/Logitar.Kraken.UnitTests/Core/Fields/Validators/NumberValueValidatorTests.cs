using Logitar.Kraken.Core.Fields.Properties;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Logitar.Kraken.Core.Fields.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class NumberValueValidatorTests
{
  private const string PropertyName = "WordCount";

  private readonly CancellationToken _cancellationToken = default;

  private readonly NumberProperties _properties = new(minimumValue: 0, maximumValue: byte.MaxValue);
  private readonly NumberValueValidator _validator;

  public NumberValueValidatorTests()
  {
    _validator = new(_properties);
  }

  [Fact(DisplayName = "Validation should fail when the value is not a valid number.")]
  public async Task Given_NotNumber_When_ValidateAsync_Then_FailureResult()
  {
    string value = "invalid";
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "NumberValueValidator" && e.ErrorMessage == "The value is not a valid number."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName);
  }

  [Fact(DisplayName = "Validation should fail when the value is too high.")]
  public async Task Given_TooHigh_When_ValidateAsync_Then_FailureResult()
  {
    string value = "999.99";
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "LessThanOrEqualValidator" && e.ErrorMessage == "The value must be less than or equal to 255."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName && HasProperty(e.CustomState, "MaximumValue", _properties.MaximumValue));
  }

  [Fact(DisplayName = "Validation should fail when the value is too low.")]
  public async Task Given_TooLow_When_ValidateAsync_Then_FailureResult()
  {
    string value = "-1.0";
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.ErrorMessage == "The value must be greater than or equal to 0."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName && HasProperty(e.CustomState, "MinimumValue", _properties.MinimumValue));
  }

  [Fact(DisplayName = "Validation should succeed when the value is valid.")]
  public async Task Given_ValidValue_When_ValidateAsync_Then_SuccessResult()
  {
    string value = "127.999";
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
