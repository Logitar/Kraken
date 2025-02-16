using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class UserRoles
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenContext.UserRoles), alias: null);

  public static readonly ColumnId UserId = new(nameof(UserRoleEntity.UserId), Table);
  public static readonly ColumnId RoleId = new(nameof(UserRoleEntity.RoleId), Table);
}
