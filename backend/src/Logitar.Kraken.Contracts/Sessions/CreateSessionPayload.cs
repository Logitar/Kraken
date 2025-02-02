namespace Logitar.Kraken.Contracts.Sessions;

public record CreateSessionPayload
{
  public Guid? Id { get; set; }

  public string User { get; set; } = string.Empty;
  public bool IsPersistent { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];

  public CreateSessionPayload()
  {
  }

  public CreateSessionPayload(string user, bool isPersistent, IEnumerable<CustomAttributeModel> customAttributes)
  {
    User = user;
    IsPersistent = isPersistent;
    CustomAttributes.AddRange(customAttributes);
  }
}
