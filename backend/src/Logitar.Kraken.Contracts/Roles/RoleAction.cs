namespace Logitar.Kraken.Contracts.Roles;

public record RoleAction
{
  public string Role { get; set; } = string.Empty;
  public CollectionAction Action { get; set; }
}
