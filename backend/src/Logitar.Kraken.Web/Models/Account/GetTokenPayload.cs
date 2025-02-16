namespace Logitar.Kraken.Web.Models.Account;

public record GetTokenPayload
{
  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }

  public string? Username { get; set; }
  public string? Password { get; set; }

  public GetTokenPayload() : base()
  {
  }

  public GetTokenPayload(string refreshToken)
  {
    RefreshToken = refreshToken;
  }

  public GetTokenPayload(string username, string password)
  {
    Username = username;
    Password = password;
  }
}
