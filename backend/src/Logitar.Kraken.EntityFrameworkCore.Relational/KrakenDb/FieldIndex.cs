using Logitar.Data;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

public static class FieldIndex
{
  public static readonly TableId Table = new(Schemas.Cms, nameof(KrakenContext.FieldIndex), alias: null);

  public static readonly ColumnId ContentId = new(nameof(FieldIndexEntity.ContentId), Table);
  public static readonly ColumnId ContentLocaleId = new(nameof(FieldIndexEntity.ContentLocaleId), Table);
  public static readonly ColumnId ContentLocaleName = new(nameof(FieldIndexEntity.ContentLocaleName), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(FieldIndexEntity.ContentTypeId), Table);
  public static readonly ColumnId ContentTypeName = new(nameof(FieldIndexEntity.ContentTypeName), Table);
  public static readonly ColumnId ContentTypeUid = new(nameof(FieldIndexEntity.ContentTypeUid), Table);
  public static readonly ColumnId ContentUid = new(nameof(FieldIndexEntity.ContentUid), Table);
  public static readonly ColumnId FieldDefinitionId = new(nameof(FieldIndexEntity.FieldDefinitionId), Table);
  public static readonly ColumnId FieldDefinitionName = new(nameof(FieldIndexEntity.FieldTypeName), Table);
  public static readonly ColumnId FieldDefinitionUid = new(nameof(FieldIndexEntity.FieldDefinitionUid), Table);
  public static readonly ColumnId FieldIndexId = new(nameof(FieldIndexEntity.FieldIndexId), Table);
  public static readonly ColumnId FieldTypeId = new(nameof(FieldIndexEntity.FieldTypeId), Table);
  public static readonly ColumnId FieldTypeName = new(nameof(FieldIndexEntity.FieldTypeName), Table);
  public static readonly ColumnId FieldTypeUid = new(nameof(FieldIndexEntity.FieldTypeUid), Table);
  public static readonly ColumnId LanguageCode = new(nameof(FieldIndexEntity.LanguageCode), Table);
  public static readonly ColumnId LanguageId = new(nameof(FieldIndexEntity.LanguageId), Table);
  public static readonly ColumnId LanguageIsDefault = new(nameof(FieldIndexEntity.LanguageIsDefault), Table);
  public static readonly ColumnId LanguageUid = new(nameof(FieldIndexEntity.LanguageUid), Table);
  public static readonly ColumnId Revision = new(nameof(FieldIndexEntity.Revision), Table);
  public static readonly ColumnId Status = new(nameof(FieldIndexEntity.Status), Table);

  public static readonly ColumnId Boolean = new(nameof(FieldIndexEntity.Boolean), Table);
  public static readonly ColumnId DateTime = new(nameof(FieldIndexEntity.DateTime), Table);
  public static readonly ColumnId Number = new(nameof(FieldIndexEntity.Number), Table);
  public static readonly ColumnId RelatedContent = new(nameof(FieldIndexEntity.RelatedContent), Table);
  public static readonly ColumnId RichText = new(nameof(FieldIndexEntity.RichText), Table);
  public static readonly ColumnId Select = new(nameof(FieldIndexEntity.Select), Table);
  public static readonly ColumnId String = new(nameof(FieldIndexEntity.String), Table);
  public static readonly ColumnId Tags = new(nameof(FieldIndexEntity.Tags), Table);
}
