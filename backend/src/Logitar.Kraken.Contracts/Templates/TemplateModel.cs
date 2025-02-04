using Logitar.Kraken.Contracts.Realms;

namespace Logitar.Kraken.Contracts.Templates;

public class TemplateModel : AggregateModel
{
  public string UniqueKey { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; } = string.Empty;
  public TemplateContentModel Content { get; set; } = new();

  public RealmModel? Realm { get; set; }
}
