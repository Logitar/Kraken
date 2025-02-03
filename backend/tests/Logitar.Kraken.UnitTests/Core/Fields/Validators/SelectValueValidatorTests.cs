using FluentValidation.Results;
using Logitar.Kraken.Core.Fields.Properties;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Logitar.Kraken.Core.Fields.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class SelectValueValidatorTests
{
  private const string PropertyName = "Category";

  private readonly CancellationToken _cancellationToken = default;

  private readonly SelectProperties _properties = new(isMultiple: false, options:
  [
    new SelectOption("linux_sysadmin"),
    new SelectOption("Software Architecture", value: "software-architecture")
  ]);
  private readonly SelectValueValidator _validator;

  public SelectValueValidatorTests()
  {
    _validator = new(_properties);
  }

  [Theory(DisplayName = "Validation should fail when the values are not valid.")]
  [InlineData("")]
  [InlineData("    ")]
  [InlineData("linux_sysadmin")]
  [InlineData(@"[linux_sysadmin]")]
  public async Task Given_InvalidValues_When_ValidateAsync_Then_FailureResult(string value)
  {
    SelectProperties properties = new(isMultiple: true, _properties.Options);
    SelectValueValidator validator = new(properties);

    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);

    ValidationFailure failure = Assert.Single(result.Errors);
    Assert.Equal(value, failure.AttemptedValue);
    Assert.Equal("SelectValueValidator", failure.ErrorCode);
    Assert.Equal("The value must be a JSON-serialized string array.", failure.ErrorMessage);
    Assert.Equal(PropertyName, failure.PropertyName);
  }

  [Fact(DisplayName = "Validation should fail when values are not allowed.")]
  public async Task Given_NotAllowed_When_ValidateAsync_Then_FailureResult()
  {
    SelectProperties properties = new(isMultiple: true, _properties.Options);
    SelectValueValidator validator = new(properties);

    string value = @"[""linux_sysadmin"",""software-architecture"",""not_allowed"",""hello-world""]";
    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    string customState = @"{""AllowedValues"":[""linux_sysadmin"",""software-architecture""]}";
    Assert.Contains(result.Errors, e => e.ErrorCode == "OptionValidator" && e.ErrorMessage == "The value should be one of the following: linux_sysadmin, software-architecture."
      && e.AttemptedValue.Equals("not_allowed") && e.PropertyName == PropertyName && customState == JsonSerializer.Serialize(e.CustomState, e.CustomState.GetType()));
    Assert.Contains(result.Errors, e => e.ErrorCode == "OptionValidator" && e.ErrorMessage == "The value should be one of the following: linux_sysadmin, software-architecture."
      && e.AttemptedValue.Equals("hello-world") && e.PropertyName == PropertyName && customState == JsonSerializer.Serialize(e.CustomState, e.CustomState.GetType()));
  }

  [Fact(DisplayName = "Validation should succeed when the value is empty.")]
  public async Task Given_Empty_When_ValidateAsync_Then_FailureResult()
  {
    SelectProperties properties = new(isMultiple: true, _properties.Options);
    SelectValueValidator validator = new(properties);

    string value = " [  ] ";
    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.True(result.IsValid);
  }

  [Theory(DisplayName = "Validation should succeed when the value is valid.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_ValidValue_When_ValidateAsync_Then_SuccessResult(bool isMultiple)
  {
    string value = "linux_sysadmin";

    SelectValueValidator validator = _validator;
    if (isMultiple)
    {
      SelectProperties properties = new(isMultiple: true, _properties.Options);
      validator = new(properties);

      value = @"[ ""software-architecture"", ""linux_sysadmin"" ]";
    }

    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.True(result.IsValid);
  }
}
