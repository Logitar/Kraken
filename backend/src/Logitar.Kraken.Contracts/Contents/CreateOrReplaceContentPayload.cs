using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Contracts.Contents;

public record CreateOrReplaceContentPayload
{
  public Guid? ContentTypeId { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<FieldValue> FieldValues { get; set; } = [];
}
