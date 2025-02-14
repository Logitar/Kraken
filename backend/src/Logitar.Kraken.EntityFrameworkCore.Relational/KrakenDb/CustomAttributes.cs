using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class CustomAttributes
{
  public static readonly TableId Table = new(Schemas.Kraken, nameof(KrakenContext.CustomAttributes), alias: null);

  public static readonly ColumnId CustomAttributeId = new(nameof(CustomAttributeEntity.CustomAttributeId), Table);
  public static readonly ColumnId EntityId = new(nameof(CustomAttributeEntity.EntityId), Table);
  public static readonly ColumnId EntityType = new(nameof(CustomAttributeEntity.EntityType), Table);
  public static readonly ColumnId Key = new(nameof(CustomAttributeEntity.Key), Table);
  public static readonly ColumnId Value = new(nameof(CustomAttributeEntity.Value), Table);
  public static readonly ColumnId ValueShortened = new(nameof(CustomAttributeEntity.ValueShortened), Table);
}
