using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class ContentLocales
{
  public static readonly TableId Table = new(Schemas.Cms, nameof(KrakenContext.ContentLocales), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(ContentLocaleEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ContentLocaleEntity.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ContentLocaleEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ContentLocaleEntity.UpdatedOn), Table);

  public static readonly ColumnId ContentId = new(nameof(ContentLocaleEntity.ContentId), Table);
  public static readonly ColumnId ContentLocaleId = new(nameof(ContentLocaleEntity.ContentLocaleId), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(ContentLocaleEntity.ContentTypeId), Table);
  public static readonly ColumnId Description = new(nameof(ContentLocaleEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(ContentLocaleEntity.DisplayName), Table);
  public static readonly ColumnId FieldValues = new(nameof(ContentLocaleEntity.FieldValues), Table);
  public static readonly ColumnId LanguageId = new(nameof(ContentLocaleEntity.LanguageId), Table);
  public static readonly ColumnId PublishedRevision = new(nameof(ContentLocaleEntity.PublishedRevision), Table);
  public static readonly ColumnId Revision = new(nameof(ContentLocaleEntity.Revision), Table);
  public static readonly ColumnId UniqueName = new(nameof(ContentLocaleEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(ContentLocaleEntity.UniqueNameNormalized), Table);
}
