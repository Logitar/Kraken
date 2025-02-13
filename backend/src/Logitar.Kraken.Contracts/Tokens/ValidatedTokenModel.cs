using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Contracts.Tokens;

public record ValidatedTokenModel
{
  public string? Subject { get; set; }
  public EmailModel? Email { get; set; }
  public List<ClaimModel> Claims { get; set; } = [];
}
