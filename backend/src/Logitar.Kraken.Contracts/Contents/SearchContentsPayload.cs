using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Contracts.Contents;

public record SearchContentsPayload : SearchPayload
{
  public Guid? ContentTypeId { get; set; }
  public Guid? LanguageId { get; set; }

  public new List<ContentSortOption> Sort { get; set; } = [];
}
