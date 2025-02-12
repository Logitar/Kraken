namespace Logitar.Kraken.Contracts.Users;

public record ResetUserPasswordPayload
{
  public string Password { get; set; } = string.Empty;

  public ResetUserPasswordPayload()
  {
  }

  public ResetUserPasswordPayload(string password)
  {
    Password = password;
  }
}
