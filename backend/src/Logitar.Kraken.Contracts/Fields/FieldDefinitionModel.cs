namespace Logitar.Kraken.Contracts.Fields;

public class FieldDefinitionModel
{
  public Guid Id { get; set; }

  public int Order { get; set; }

  public FieldTypeModel FieldType { get; set; } = new();

  public bool IsInvariant { get; set; }
  public bool IsRequired { get; set; }
  public bool IsIndexed { get; set; }
  public bool IsUnique { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
  public string? Placeholder { get; set; }

  public override bool Equals(object? obj) => obj is FieldDefinitionModel field && field.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";
}
