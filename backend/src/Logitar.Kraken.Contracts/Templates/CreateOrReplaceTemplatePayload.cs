namespace Logitar.Kraken.Contracts.Templates;

public record CreateOrReplaceTemplatePayload
{
  public string UniqueKey { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; } = string.Empty;
  public TemplateContentModel Content { get; set; } = new();
}
