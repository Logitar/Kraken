using FluentValidation;
using System.Net.Mime; // NOTE(fpion): cannot be added to CSPROJ due to ContentType aggregate.

namespace Logitar.Kraken.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTypeValidatorTests
{
  private readonly ValidationContext<ContentTypeValidatorTests> _context;
  private readonly ContentTypeValidator<ContentTypeValidatorTests> _validator = new();

  public ContentTypeValidatorTests()
  {
    _context = new ValidationContext<ContentTypeValidatorTests>(this);
  }

  [Fact(DisplayName = "GetDefaultMessageTemplate: it should return the correct default message template.")]
  public void Given_Validator_When_GetDefaultMessageTemplate_Then_CorrectMessage()
  {
    string expected = "'{PropertyName}' must be one of the following: text/plain.";
    Assert.Equal(expected, _validator.GetDefaultMessageTemplate(errorCode: string.Empty));
  }

  [Fact(DisplayName = "IsValid: it should return false when the content type is not allowed.")]
  public void Given_NotAllowed_When_IsValid_Then_FalseReturned()
  {
    string value = MediaTypeNames.Text.Html;
    Assert.False(_validator.IsValid(_context, value));
  }

  [Fact(DisplayName = "IsValid: it should return true when the content type is allowed.")]
  public void Given_Allowed_When_IsValid_Then_TrueReturned()
  {
    Assert.True(_validator.IsValid(_context, MediaTypeNames.Text.Plain.ToUpperInvariant()));
  }
}
