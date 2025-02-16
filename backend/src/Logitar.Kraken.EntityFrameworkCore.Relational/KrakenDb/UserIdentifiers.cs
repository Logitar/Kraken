using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class UserIdentifiers
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenContext.UserIdentifiers), alias: null);

  public static readonly ColumnId Key = new(nameof(UserIdentifierEntity.Key), Table);
  public static readonly ColumnId RealmId = new(nameof(UserIdentifierEntity.RealmId), Table);
  public static readonly ColumnId UserId = new(nameof(UserIdentifierEntity.UserId), Table);
  public static readonly ColumnId UserIdentifierId = new(nameof(UserIdentifierEntity.UserIdentifierId), Table);
  public static readonly ColumnId Value = new(nameof(UserIdentifierEntity.Value), Table);
}
