namespace Logitar.Kraken.Contracts.Sessions;

public record RenewSessionPayload
{
  public string RefreshToken { get; set; } = string.Empty;

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];

  public RenewSessionPayload()
  {
  }

  public RenewSessionPayload(string refreshToken, IEnumerable<CustomAttributeModel> customAttributes)
  {
    RefreshToken = refreshToken;
    CustomAttributes.AddRange(customAttributes);
  }
}
