using FluentValidation;
using System.Net.Mime; // NOTE(fpion): cannot be added to CSPROJ due to ContentType aggregate.

namespace Logitar.Kraken.Core.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class TemplateContentTypeValidatorTests
{
  private readonly ValidationContext<TemplateContentTypeValidatorTests> _context;
  private readonly TemplateContentTypeValidator<TemplateContentTypeValidatorTests> _validator = new();

  public TemplateContentTypeValidatorTests()
  {
    _context = new ValidationContext<TemplateContentTypeValidatorTests>(this);
  }

  [Fact(DisplayName = "GetDefaultMessageTemplate: it should return the correct default message template.")]
  public void Given_Validator_When_GetDefaultMessageTemplate_Then_CorrectMessage()
  {
    string expected = "'{PropertyName}' must be one of the following: text/plain, text/html.";
    Assert.Equal(expected, _validator.GetDefaultMessageTemplate(errorCode: string.Empty));
  }

  [Fact(DisplayName = "IsValid: it should return false when the content type is not allowed.")]
  public void Given_NotAllowed_When_IsValid_Then_FalseReturned()
  {
    string value = MediaTypeNames.Text.Markdown;
    Assert.False(_validator.IsValid(_context, value));
  }

  [Fact(DisplayName = "IsValid: it should return true when the content type is allowed.")]
  public void Given_Allowed_When_IsValid_Then_TrueReturned()
  {
    Assert.True(_validator.IsValid(_context, MediaTypeNames.Text.Html.ToUpperInvariant()));
  }
}
