using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class ApiKeyRoles
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenContext.ApiKeyRoles), alias: null);

  public static readonly ColumnId ApiKeyId = new(nameof(ApiKeyRoleEntity.ApiKeyId), Table);
  public static readonly ColumnId RoleId = new(nameof(ApiKeyRoleEntity.RoleId), Table);
}
