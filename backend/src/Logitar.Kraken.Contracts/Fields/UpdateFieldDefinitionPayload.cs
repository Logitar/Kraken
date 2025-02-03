namespace Logitar.Kraken.Contracts.Fields;

public record UpdateFieldDefinitionPayload
{
  public bool? IsInvariant { get; set; }
  public bool? IsRequired { get; set; }
  public bool? IsIndexed { get; set; }
  public bool? IsUnique { get; set; }

  public string? UniqueName { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }
  public ChangeModel<string>? Placeholder { get; set; }
}
