using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Contracts.Localization;

namespace Logitar.Kraken.Contracts.Contents;

public record PublishedContentLocale
{
  public PublishedContent Content { get; set; }
  public LanguageSummary? Language { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<FieldValue> FieldValues { get; set; } = [];

  public long Revision { get; set; }
  public ActorModel PublishedBy { get; set; } = new();
  public DateTime PublishedOn { get; set; }

  public PublishedContentLocale(PublishedContent content)
  {
    Content = content;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName}";
}
