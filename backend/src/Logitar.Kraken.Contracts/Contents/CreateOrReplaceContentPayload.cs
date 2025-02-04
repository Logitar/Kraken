using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Contracts.Contents;

public record CreateOrReplaceContentPayload
{
  public Guid? ContentTypeId { get; set; } // ISSUE #38: https://github.com/Logitar/Kraken/issues/38

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<FieldValue> FieldValues { get; set; } = [];
}
