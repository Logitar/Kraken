namespace Logitar.Kraken.Contracts.Sessions;

public record SignInSessionPayload
{
  public Guid? Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public bool IsPersistent { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];

  public SignInSessionPayload()
  {
  }

  public SignInSessionPayload(string uniqueName, string password, bool isPersistent, IEnumerable<CustomAttributeModel> customAttributes)
  {
    UniqueName = uniqueName;
    Password = password;
    IsPersistent = isPersistent;
    CustomAttributes.AddRange(customAttributes);
  }
}
