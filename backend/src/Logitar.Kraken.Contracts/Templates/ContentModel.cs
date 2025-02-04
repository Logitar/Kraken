namespace Logitar.Kraken.Contracts.Templates;

public record TemplateContentModel : ITemplateContent
{
  public string Type { get; set; } = string.Empty;
  public string Text { get; set; } = string.Empty;

  public TemplateContentModel()
  {
  }

  public TemplateContentModel(ITemplateContent content) : this(content.Type, content.Text)
  {
  }

  public TemplateContentModel(string type, string text)
  {
    Type = type;
    Text = text;
  }

  public static TemplateContentModel Html(string text) => new(MediaTypeNames.Text.Html, text);
  public static TemplateContentModel PlainText(string text) => new(MediaTypeNames.Text.Plain, text);
}
