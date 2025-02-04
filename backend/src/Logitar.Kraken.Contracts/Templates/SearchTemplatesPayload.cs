using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Templates;

public record SearchTemplatesPayload : SearchPayload
{
  public string? ContentType { get; set; }

  public new List<TemplateSortOption> Sort { get; set; } = [];
}
