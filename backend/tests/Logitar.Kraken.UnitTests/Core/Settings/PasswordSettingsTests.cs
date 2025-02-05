using FluentValidation;
using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class PasswordSettingsTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    PasswordSettingsModel model = new();

    PasswordSettings settings = new(model);

    Assert.Equal(model.RequiredLength, settings.RequiredLength);
    Assert.Equal(model.RequiredUniqueChars, settings.RequiredUniqueChars);
    Assert.Equal(model.RequireNonAlphanumeric, settings.RequireNonAlphanumeric);
    Assert.Equal(model.RequireLowercase, settings.RequireLowercase);
    Assert.Equal(model.RequireUppercase, settings.RequireUppercase);
    Assert.Equal(model.RequireDigit, settings.RequireDigit);
    Assert.Equal(model.HashingStrategy, settings.HashingStrategy);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    PasswordSettings settings = new();
    Assert.Equal(8, settings.RequiredLength);
    Assert.Equal(8, settings.RequiredUniqueChars);
    Assert.True(settings.RequireNonAlphanumeric);
    Assert.True(settings.RequireLowercase);
    Assert.True(settings.RequireUppercase);
    Assert.True(settings.RequireDigit);
    Assert.Equal("PBKDF2", settings.HashingStrategy);

    settings = new(requiredLength: 6, requiredUniqueChars: 3, requireNonAlphanumeric: false, requireLowercase: true, requireUppercase: true, requireDigit: true, hashingStrategy: "MD5");
    Assert.Equal(6, settings.RequiredLength);
    Assert.Equal(3, settings.RequiredUniqueChars);
    Assert.False(settings.RequireNonAlphanumeric);
    Assert.True(settings.RequireLowercase);
    Assert.True(settings.RequireUppercase);
    Assert.True(settings.RequireDigit);
    Assert.Equal("MD5", settings.HashingStrategy);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_Invalid_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<ValidationException>(() => new PasswordSettings(
      requiredLength: 2,
      requiredUniqueChars: 3,
      requireNonAlphanumeric: false,
      requireLowercase: true,
      requireUppercase: true,
      requireDigit: true,
      hashingStrategy: "    "));
    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "RequiredLength");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LessThanOrEqualValidator" && e.PropertyName == "RequiredUniqueChars");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "HashingStrategy");
  }
}
