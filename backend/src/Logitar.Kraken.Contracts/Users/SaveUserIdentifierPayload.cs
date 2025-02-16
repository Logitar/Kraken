namespace Logitar.Kraken.Contracts.Users;

public record SaveUserIdentifierPayload
{
  public string Value { get; set; } = string.Empty;

  public SaveUserIdentifierPayload()
  {
  }

  public SaveUserIdentifierPayload(string value)
  {
    Value = value;
  }
}
