namespace Logitar.Kraken.Contracts.Fields;

public record CreateOrReplaceFieldDefinitionPayload
{
  public Guid? FieldTypeId { get; set; }

  public bool IsInvariant { get; set; }
  public bool IsRequired { get; set; }
  public bool IsIndexed { get; set; }
  public bool IsUnique { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
  public string? Placeholder { get; set; }
}
