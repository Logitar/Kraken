namespace Logitar.Kraken.Contracts.Passwords;

public record ValidateOneTimePasswordPayload
{
  public string Password { get; set; } = string.Empty;
  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];
}
