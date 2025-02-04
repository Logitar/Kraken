using FluentValidation;
using Logitar.Kraken.Contracts.Templates;
using System.Net.Mime; // NOTE(fpion): cannot be added to CSPROJ due to ContentType aggregate.

namespace Logitar.Kraken.Core.Templates;

[Trait(Traits.Category, Categories.Unit)]
public class TemplateContentTests
{
  [Fact(DisplayName = "Create: it should create a new instance using the same type.")]
  public void Given_Text_When_Create_Then_SameType()
  {
    TemplateContent content = TemplateContent.PlainText("old text");
    TemplateContent created = content.Create("new text");

    Assert.Equal(content.Type, created.Type);
    Assert.NotEqual(content.Text, created.Text);
  }

  [Fact(DisplayName = "Html: it should create text/html template contents.")]
  public void Given_Text_When_Html_Then_HtmlContent()
  {
    string text = "<p>Hello World!</p>";
    TemplateContent content = TemplateContent.Html(text);
    Assert.Equal(MediaTypeNames.Text.Html, content.Type);
    Assert.Equal(text, content.Text);
  }

  [Fact(DisplayName = "It should construct the correct template contents.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    string type = $"  {MediaTypeNames.Text.Plain}  ";
    string text = "  Hello World!  ";
    TemplateContentModel model = new(type, text);

    TemplateContent content = new(model);

    Assert.Equal(type.Trim(), content.Type);
    Assert.Equal(text.Trim(), content.Text);
  }

  [Fact(DisplayName = "It should throw TemplateContentValidator when the arguments are not valid.")]
  public void Given_NotValid_When_ctor_Then_TemplateContentValidator()
  {
    TemplateContentModel content = new("html", text: string.Empty);
    var exception = Assert.Throws<ValidationException>(() => new TemplateContent(content));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "ContentTypeValidator" && e.PropertyName == "Type");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Text");
  }

  [Fact(DisplayName = "PlainText: it should create text/plain template contents.")]
  public void Given_Text_When_PlainText_Then_PlainTextContent()
  {
    string text = "Hello World!";
    TemplateContent content = TemplateContent.PlainText(text);
    Assert.Equal(MediaTypeNames.Text.Plain, content.Type);
    Assert.Equal(text, content.Text);
  }
}
