using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Contracts.Contents;

public record UpdateContentPayload
{
  public string? UniqueName { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }

  public List<FieldValueUpdate> FieldValues { get; set; } = [];
}
