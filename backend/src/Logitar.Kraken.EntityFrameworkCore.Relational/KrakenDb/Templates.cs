using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class Templates
{
  public static readonly TableId Table = new(Schemas.Messaging, nameof(KrakenContext.Templates), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(TemplateEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(TemplateEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(TemplateEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(TemplateEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(TemplateEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(TemplateEntity.Version), Table);

  public static readonly ColumnId ContentText = new(nameof(TemplateEntity.ContentText), Table);
  public static readonly ColumnId ContentType = new(nameof(TemplateEntity.ContentType), Table);
  public static readonly ColumnId Description = new(nameof(TemplateEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(TemplateEntity.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(TemplateEntity.Id), Table);
  public static readonly ColumnId RealmId = new(nameof(TemplateEntity.RealmId), Table);
  public static readonly ColumnId Subject = new(nameof(TemplateEntity.Subject), Table);
  public static readonly ColumnId TemplateId = new(nameof(TemplateEntity.TemplateId), Table);
  public static readonly ColumnId UniqueKey = new(nameof(TemplateEntity.UniqueKey), Table);
  public static readonly ColumnId UniqueKeyNormalized = new(nameof(TemplateEntity.UniqueKeyNormalized), Table);
}
