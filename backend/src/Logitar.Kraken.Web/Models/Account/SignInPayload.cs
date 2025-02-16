namespace Logitar.Kraken.Web.Models.Account;

public record SignInPayload
{
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;

  public SignInPayload()
  {
  }

  public SignInPayload(string username, string password)
  {
    Username = username;
    Password = password;
  }
}
