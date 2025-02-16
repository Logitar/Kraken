using FluentValidation;

namespace Logitar.Kraken.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class PhoneNumberValidatorTests
{
  private readonly ValidationContext<PhoneNumberValidatorTests> _context;
  private readonly PhoneNumberValidator<PhoneNumberValidatorTests> _validator = new();

  public PhoneNumberValidatorTests()
  {
    _context = new ValidationContext<PhoneNumberValidatorTests>(this);
  }

  [Theory(DisplayName = "IsValid: it should return false when the value is not a valid phone number.")]
  [InlineData("")]
  [InlineData("    ")]
  [InlineData("invalid")]
  [InlineData("12345678901234567890")]
  public void Given_Invalid_When_IsValid_Then_FalseReturned(string value)
  {
    Assert.False(_validator.IsValid(_context, value));
  }

  [Theory(DisplayName = "IsValid: it should return true when the value is a valid phone number.")]
  [InlineData("+15148454636")]
  [InlineData("(33 1) 40 15 38 59")]
  public void Given_PhoneNumber_When_IsValid_Then_TrueReturned(string value)
  {
    Assert.True(_validator.IsValid(_context, value));
  }
}
