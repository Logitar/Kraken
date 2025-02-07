using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class FieldTypes
{
  public static readonly TableId Table = new(Schemas.Cms, nameof(KrakenContext.FieldTypes), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(FieldTypeEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(FieldTypeEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(FieldTypeEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(FieldTypeEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(FieldTypeEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(FieldTypeEntity.Version), Table);

  public static readonly ColumnId DataType = new(nameof(FieldTypeEntity.DataType), Table);
  public static readonly ColumnId Description = new(nameof(FieldTypeEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(FieldTypeEntity.DisplayName), Table);
  public static readonly ColumnId FieldTypeId = new(nameof(FieldTypeEntity.FieldTypeId), Table);
  public static readonly ColumnId Id = new(nameof(FieldTypeEntity.Id), Table);
  public static readonly ColumnId Properties = new(nameof(FieldTypeEntity.Properties), Table);
  public static readonly ColumnId RealmId = new(nameof(FieldTypeEntity.RealmId), Table);
  public static readonly ColumnId UniqueName = new(nameof(FieldTypeEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(FieldTypeEntity.UniqueNameNormalized), Table);
}
