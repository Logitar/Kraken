using FluentValidation;
using FluentValidation.Results;

namespace Logitar.Kraken.Core.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class SecretTests
{
  [Fact(DisplayName = "ctor: it should construct the correct instance given a valid value.")]
  public void Given_ValidValue_When_ctor_Then_ConstructedCorrectly()
  {
    string value = "  S3cR3+!$  ";
    Secret secret = new(value);
    Assert.Equal(value.Trim(), secret.Value);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException given an empty or white-space value.")]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyOrWhiteSpace_When_ctor_Then_ValidationException(string value)
  {
    var exception = Assert.Throws<ValidationException>(() => new Secret(value));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Secret_When_ToString_Then_CorrectString()
  {
    Secret secret = new("S3cR3+!$");
    Assert.Equal(secret.Value, secret.ToString());
  }

  [Fact(DisplayName = "TryCreate: it should return a new instance given a valid value.")]
  public void Given_ValidValue_When_TryCreate_Then_InstanceReturned()
  {
    string value = "  S3cR3+!$  ";
    Secret? secret = Secret.TryCreate(value);
    Assert.NotNull(secret);
    Assert.Equal(value.Trim(), secret.Value);
  }

  [Theory(DisplayName = "TryCreate: it should return null given a null, empty, or white-space value.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_NullEmptyOrWhiteSpace_When_TryCreate_Then_NullReturned(string? value)
  {
    Assert.Null(Secret.TryCreate(value));
  }
}
