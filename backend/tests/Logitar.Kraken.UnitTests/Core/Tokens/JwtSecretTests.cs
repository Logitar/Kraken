using FluentValidation;
using FluentValidation.Results;
using Logitar.Security.Cryptography;

namespace Logitar.Kraken.Core.Tokens;

[Trait(Traits.Category, Categories.Unit)]
public class JwtSecretTests
{
  [Fact(DisplayName = "CreateOrGenerate: it should return a new instance given a valid value.")]
  public void Given_ValidValue_When_CreateOrGenerate_Then_InstanceReturned()
  {
    string value = "  NHR6jTEL9fU8aZ3CswqA5cYFyp7eBbdv  ";
    JwtSecret? secret = JwtSecret.CreateOrGenerate(value);
    Assert.NotNull(secret);
    Assert.Equal(value.Trim(), secret.Value);
  }

  [Theory(DisplayName = "CreateOrGenerate: it should return a new generated value given a null, empty, or white-space value.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_NullEmptyOrWhiteSpace_When_CreateOrGenerate_Then_RandomGenerated(string? value)
  {
    JwtSecret secret = JwtSecret.CreateOrGenerate(value);
    Assert.Equal(JwtSecret.MinimumLength, secret.Value.Length);
  }

  [Fact(DisplayName = "CreateOrGenerate: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_CreateOrGenerate_Then_ValidationException()
  {
    string value = RandomStringGenerator.GetString(999);
    var exception = Assert.Throws<ValidationException>(() => JwtSecret.CreateOrGenerate(value));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }

  [Fact(DisplayName = "ctor: it should construct the correct instance given a valid value.")]
  public void Given_ValidValue_When_ctor_Then_ConstructedCorrectly()
  {
    string value = "  bnXkytd^:PfcB;xL4,hW+TJZg5G#{u`@  ";
    JwtSecret secret = new(value);
    Assert.Equal(value.Trim(), secret.Value);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException given an empty, or white-space value.")]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_EmptyOrWhiteSpace_When_ctor_Then_ValidationException(string value)
  {
    var exception = Assert.Throws<ValidationException>(() => new JwtSecret(value));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MinimumLengthValidator" && e.PropertyName == "Value");
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException given an invalid value.")]
  public void Given_InvalidValue_When_ctor_Then_ValidationException()
  {
    string value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    var exception = Assert.Throws<ValidationException>(() => new JwtSecret(value));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("MinimumLengthValidator", failure.ErrorCode);
    Assert.Equal("Value", failure.PropertyName);
  }

  [Fact(DisplayName = "Generate: it should generate a secret of the minimum length.")]
  public void Given_NoLength_When_Generate_Then_MinimumLength()
  {
    JwtSecret secret = JwtSecret.Generate();
    Assert.Equal(JwtSecret.MinimumLength, secret.Value.Length);
  }

  [Fact(DisplayName = "Generate: it should generate a secret of the specified length.")]
  public void Given_Length_When_Generate_Then_SpecifiedLength()
  {
    int length = 42;
    JwtSecret secret = JwtSecret.Generate(length);
    Assert.Equal(length, secret.Value.Length);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_JwtSecret_When_ToString_Then_CorrectString()
  {
    JwtSecret secret = new("sGFbu9kMr7LwEz3jAYBt6CUHKcQpyd5T");
    Assert.Equal(secret.Value, secret.ToString());
  }
}
