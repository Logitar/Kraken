using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Messages
{
  public static readonly TableId Table = new(Schemas.Messaging, nameof(KrakenContext.Messages), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(MessageEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(MessageEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(MessageEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(MessageEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(MessageEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(MessageEntity.Version), Table);

  public static readonly ColumnId BodyText = new(nameof(MessageEntity.BodyText), Table);
  public static readonly ColumnId BodyType = new(nameof(MessageEntity.BodyType), Table);
  public static readonly ColumnId Id = new(nameof(MessageEntity.Id), Table);
  public static readonly ColumnId IgnoreUserLocale = new(nameof(MessageEntity.IgnoreUserLocale), Table);
  public static readonly ColumnId IsDemo = new(nameof(MessageEntity.IsDemo), Table);
  public static readonly ColumnId Locale = new(nameof(MessageEntity.Locale), Table);
  public static readonly ColumnId MessageId = new(nameof(MessageEntity.MessageId), Table);
  public static readonly ColumnId RealmId = new(nameof(MessageEntity.RealmId), Table);
  public static readonly ColumnId ResultData = new(nameof(MessageEntity.ResultData), Table);
  public static readonly ColumnId SenderAddress = new(nameof(MessageEntity.SenderAddress), Table);
  public static readonly ColumnId SenderDisplayName = new(nameof(MessageEntity.SenderDisplayName), Table);
  public static readonly ColumnId SenderId = new(nameof(MessageEntity.SenderId), Table);
  public static readonly ColumnId SenderIsDefault = new(nameof(MessageEntity.SenderIsDefault), Table);
  public static readonly ColumnId SenderProvider = new(nameof(MessageEntity.SenderProvider), Table);
  public static readonly ColumnId Status = new(nameof(MessageEntity.Status), Table);
  public static readonly ColumnId Subject = new(nameof(MessageEntity.Subject), Table);
  public static readonly ColumnId TemplateDisplayName = new(nameof(MessageEntity.TemplateDisplayName), Table);
  public static readonly ColumnId TemplateId = new(nameof(MessageEntity.TemplateId), Table);
  public static readonly ColumnId TemplateUniqueKey = new(nameof(MessageEntity.TemplateUniqueKey), Table);
  public static readonly ColumnId Variables = new(nameof(MessageEntity.Variables), Table);
}
