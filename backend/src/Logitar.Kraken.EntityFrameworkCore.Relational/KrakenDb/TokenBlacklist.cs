using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class TokenBlacklist
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenContext.TokenBlacklist), alias: null);

  public static readonly ColumnId BlacklistedTokenId = new(nameof(BlacklistedTokenEntity.BlacklistedTokenId), Table);
  public static readonly ColumnId ExpiresOn = new(nameof(BlacklistedTokenEntity.ExpiresOn), Table);
  public static readonly ColumnId TokenId = new(nameof(BlacklistedTokenEntity.TokenId), Table);
}
