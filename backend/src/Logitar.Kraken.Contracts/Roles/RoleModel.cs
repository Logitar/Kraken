namespace Logitar.Kraken.Contracts.Roles;

public class RoleModel : AggregateModel
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
