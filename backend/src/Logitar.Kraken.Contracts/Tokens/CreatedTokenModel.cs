namespace Logitar.Kraken.Contracts.Tokens;

public record CreatedTokenModel
{
  public string Token { get; set; } = string.Empty;

  public CreatedTokenModel()
  {
  }

  public CreatedTokenModel(string token)
  {
    Token = token;
  }
}
