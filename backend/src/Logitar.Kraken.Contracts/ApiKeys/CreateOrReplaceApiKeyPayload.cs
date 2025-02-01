namespace Logitar.Kraken.Contracts.ApiKeys;

public record CreateOrReplaceApiKeyPayload
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];
  public List<string> Roles { get; set; } = [];
}
