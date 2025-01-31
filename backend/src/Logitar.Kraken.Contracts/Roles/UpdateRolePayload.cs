namespace Logitar.Kraken.Contracts.Roles;

public record UpdateRolePayload
{
  public string? UniqueName { get; set; } = string.Empty;
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];
}
