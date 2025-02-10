using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Senders
{
  public static readonly TableId Table = new(Schemas.Messaging, nameof(KrakenContext.Senders), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(SenderEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(SenderEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(SenderEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(SenderEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(SenderEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(SenderEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(SenderEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(SenderEntity.DisplayName), Table);
  public static readonly ColumnId EmailAddress = new(nameof(SenderEntity.EmailAddress), Table);
  public static readonly ColumnId Id = new(nameof(SenderEntity.Id), Table);
  public static readonly ColumnId IsDefault = new(nameof(SenderEntity.IsDefault), Table);
  public static readonly ColumnId Provider = new(nameof(SenderEntity.Provider), Table);
  public static readonly ColumnId RealmId = new(nameof(SenderEntity.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(SenderEntity.RealmUid), Table);
  public static readonly ColumnId SenderId = new(nameof(SenderEntity.SenderId), Table);
  public static readonly ColumnId Settings = new(nameof(SenderEntity.Settings), Table);
  public static readonly ColumnId Type = new(nameof(SenderEntity.Type), Table);
}
