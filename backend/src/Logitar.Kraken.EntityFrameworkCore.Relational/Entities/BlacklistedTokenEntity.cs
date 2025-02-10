namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class BlacklistedTokenEntity
{
  public int BlacklistedTokenId { get; private set; }

  public string TokenId { get; private set; }

  public DateTime? ExpiresOn { get; set; }

  public BlacklistedTokenEntity(string tokenId)
  {
    TokenId = tokenId;
  }

  private BlacklistedTokenEntity() : this(string.Empty)
  {
  }

  public override bool Equals(object? obj) => obj is BlacklistedTokenEntity blacklisted && blacklisted.TokenId == TokenId;
  public override int GetHashCode() => TokenId.GetHashCode();
  public override string ToString() => $"{GetType()} (TokenId={TokenId})";
}
