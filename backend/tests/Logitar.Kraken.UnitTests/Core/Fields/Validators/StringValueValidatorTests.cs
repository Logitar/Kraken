using Bogus;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Security.Cryptography;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Logitar.Kraken.Core.Fields.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class StringValueValidatorTests
{
  private const string PropertyName = "HealthInsuranceNumber";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly StringProperties _properties = new(minimumLength: 12, maximumLength: 14, pattern: "[A-Z]{4}\\s?[0-9]{4}\\s?[0-9]{4}");
  private readonly StringValueValidator _validator;

  public StringValueValidatorTests()
  {
    _validator = new(_properties);
  }

  [Fact(DisplayName = "Validation should fail when the value does not match the pattern.")]
  public async Task Given_NotMatching_When_ValidateAsync_Then_FailureResult()
  {
    string value = new(_faker.Person.BuildHealthInsuranceNumber().Reverse().ToArray());
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "RegularExpressionValidator" && e.ErrorMessage == $"The value must match the pattern '{_properties.Pattern}'."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName && HasProperty(e.CustomState, "Pattern", _properties.Pattern));
  }

  [Fact(DisplayName = "Validation should fail when the value is too long.")]
  public async Task Given_TooLong_When_ValidateAsync_Then_FailureResult()
  {
    string value = RandomStringGenerator.GetString(99);
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.ErrorMessage == "The length of the value may not exceed 14 characters."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName && HasProperty(e.CustomState, "MaximumLength", _properties.MaximumLength));
  }

  [Fact(DisplayName = "Validation should fail when the value is too short.")]
  public async Task Given_TooShort_When_ValidateAsync_Then_FailureResult()
  {
    string value = "A";
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "MinimumLengthValidator" && e.ErrorMessage == "The length of the value must be at least 12 characters."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName && HasProperty(e.CustomState, "MinimumLength", _properties.MinimumLength));
  }

  [Fact(DisplayName = "Validation should succeed when the value is valid.")]
  public async Task Given_ValidValue_When_ValidateAsync_Then_SuccessResult()
  {
    string value = _faker.Person.BuildHealthInsuranceNumber(withSpaces: true);
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
