using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Recipients
{
  public static readonly TableId Table = new(Schemas.Messaging, nameof(KrakenContext.Recipients), alias: null);

  public static readonly ColumnId Address = new(nameof(RecipientEntity.Address), Table);
  public static readonly ColumnId DisplayName = new(nameof(RecipientEntity.DisplayName), Table);
  public static readonly ColumnId MessageId = new(nameof(RecipientEntity.MessageId), Table);
  public static readonly ColumnId PhoneNumber = new(nameof(RecipientEntity.PhoneNumber), Table);
  public static readonly ColumnId RecipientId = new(nameof(RecipientEntity.RecipientId), Table);
  public static readonly ColumnId Type = new(nameof(RecipientEntity.Type), Table);
  public static readonly ColumnId UserEmailAddress = new(nameof(RecipientEntity.UserEmailAddress), Table);
  public static readonly ColumnId UserFullName = new(nameof(RecipientEntity.UserFullName), Table);
  public static readonly ColumnId UserId = new(nameof(RecipientEntity.UserId), Table);
  public static readonly ColumnId UserPicture = new(nameof(RecipientEntity.UserPicture), Table);
  public static readonly ColumnId UserUniqueName = new(nameof(RecipientEntity.UserUniqueName), Table);
}
