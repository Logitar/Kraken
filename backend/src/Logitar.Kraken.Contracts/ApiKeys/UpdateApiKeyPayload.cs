using Logitar.Kraken.Contracts.Roles;

namespace Logitar.Kraken.Contracts.ApiKeys;

public record UpdateApiKeyPayload
{
  public string? Name { get; set; }
  public ChangeModel<string>? Description { get; set; }
  public ChangeModel<DateTime?>? ExpiresOn { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];
  public List<RoleAction> Roles { get; set; } = [];
}
