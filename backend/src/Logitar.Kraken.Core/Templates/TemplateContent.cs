using FluentValidation;
using Logitar.Kraken.Contracts.Templates;
using Logitar.Kraken.Core.Templates.Validators;
using System.Net.Mime; // NOTE(fpion): cannot be added to CSPROJ due to ContentType aggregate.

namespace Logitar.Kraken.Core.Templates;

public record TemplateContent : ITemplateContent
{
  public string Type { get; }
  public string Text { get; }

  [JsonConstructor]
  private TemplateContent(string type, string text)
  {
    Type = type.Trim();
    Text = text.Trim();
    new TemplateContentValidator().ValidateAndThrow(this);
  }

  public TemplateContent(ITemplateContent content) : this(content.Type, content.Text)
  {
  }

  public static TemplateContent Html(string text) => new(MediaTypeNames.Text.Html, text);
  public static TemplateContent PlainText(string text) => new(MediaTypeNames.Text.Plain, text);

  public TemplateContent Create(string text) => new(Type, text);
}
