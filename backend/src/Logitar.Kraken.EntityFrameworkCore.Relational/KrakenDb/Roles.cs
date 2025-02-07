using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Roles
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenContext.Roles), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(RoleEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(RoleEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(RoleEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(RoleEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(RoleEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(RoleEntity.Version), Table);

  public static readonly ColumnId CustomAttributes = new(nameof(RoleEntity.CustomAttributes), Table);
  public static readonly ColumnId Description = new(nameof(RoleEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(RoleEntity.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(RoleEntity.Id), Table);
  public static readonly ColumnId RealmId = new(nameof(RoleEntity.RealmId), Table);
  public static readonly ColumnId RoleId = new(nameof(RoleEntity.RoleId), Table);
  public static readonly ColumnId UniqueName = new(nameof(RoleEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(RoleEntity.UniqueNameNormalized), Table);
}
