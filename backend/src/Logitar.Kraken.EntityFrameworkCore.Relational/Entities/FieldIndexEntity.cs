using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class FieldIndexEntity : ISegregatedEntity
{
  public const int MaximumLength = byte.MaxValue;

  public int FieldIndexId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }
  public string RealmSlug { get; private set; } = string.Empty;

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public Guid ContentTypeUid { get; private set; }
  public string ContentTypeName { get; private set; } = string.Empty;

  public LanguageEntity? Language { get; private set; }
  public int? LanguageId { get; private set; }
  public Guid? LanguageUid { get; private set; }
  public string? LanguageCode { get; private set; }
  public bool LanguageIsDefault { get; private set; }

  public FieldTypeEntity? FieldType { get; private set; }
  public int FieldTypeId { get; private set; }
  public Guid FieldTypeUid { get; private set; }
  public string FieldTypeName { get; private set; } = string.Empty;

  public FieldDefinitionEntity? FieldDefinition { get; private set; }
  public int FieldDefinitionId { get; private set; }
  public Guid FieldDefinitionUid { get; private set; }
  public string FieldDefinitionName { get; private set; } = string.Empty;

  public ContentEntity? Content { get; private set; }
  public int ContentId { get; private set; }
  public Guid ContentUid { get; private set; }

  public ContentLocaleEntity? ContentLocale { get; private set; }
  public int ContentLocaleId { get; private set; }
  public string ContentLocaleName { get; private set; } = string.Empty;

  public long Revision { get; private set; }
  public ContentStatus Status { get; private set; }

  public bool? Boolean { get; private set; }
  public DateTime? DateTime { get; private set; }
  public double? Number { get; private set; }
  public string? RelatedContent { get; private set; }
  public string? RichText { get; private set; }
  public string? Select { get; private set; }
  public string? String { get; private set; }
  public string? Tags { get; private set; }

  public FieldIndexEntity(
    RealmEntity? realm,
    ContentTypeEntity contentType,
    LanguageEntity? language,
    FieldTypeEntity fieldType,
    FieldDefinitionEntity fieldDefinition,
    ContentEntity content,
    ContentLocaleEntity contentLocale,
    ContentStatus status,
    string value)
  {
    if (realm != null)
    {
      Realm = realm;
      RealmId = realm.RealmId;
      RealmUid = realm.Id;
      RealmSlug = realm.UniqueSlugNormalized;
    }

    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;
    ContentTypeUid = contentType.Id;
    ContentTypeName = contentType.UniqueNameNormalized;

    if (language != null)
    {
      Language = language;
      LanguageId = language.LanguageId;
      LanguageUid = language.Id;
      LanguageCode = language.CodeNormalized;
      LanguageIsDefault = language.IsDefault;
    }

    FieldType = fieldType;
    FieldTypeId = fieldType.FieldTypeId;
    FieldTypeUid = fieldType.Id;
    FieldTypeName = fieldType.UniqueNameNormalized;

    FieldDefinition = fieldDefinition;
    FieldDefinitionId = fieldDefinition.FieldDefinitionId;
    FieldDefinitionUid = fieldDefinition.Id;
    FieldDefinitionName = fieldDefinition.UniqueNameNormalized;

    Content = content;
    ContentId = content.ContentId;
    ContentUid = content.Id;

    ContentLocale = contentLocale;
    ContentLocaleId = contentLocale.ContentLocaleId;
    ContentLocaleName = contentLocale.UniqueNameNormalized;

    Status = status;

    long revision = (status == ContentStatus.Published ? contentLocale.PublishedRevision : null) ?? contentLocale.Revision;
    Update(revision, value);
  }

  private FieldIndexEntity()
  {
  }

  public void Update(long revision, string value)
  {
    if (FieldType == null)
    {
      throw new InvalidOperationException($"The {nameof(FieldType)} is required.");
    }

    Revision = revision;

    switch (FieldType.DataType)
    {
      case DataType.Boolean:
        Boolean = bool.Parse(value);
        break;
      case DataType.DateTime:
        DateTime = System.DateTime.Parse(value);
        break;
      case DataType.Number:
        Number = double.Parse(value);
        break;
      case DataType.RelatedContent:
        RelatedContent = value;
        break;
      case DataType.RichText:
        RichText = value;
        break;
      case DataType.Select:
        Select = value;
        break;
      case DataType.String:
        String = value.Truncate(MaximumLength);
        break;
      case DataType.Tags:
        Tags = value;
        break;
      default:
        throw new DataTypeNotSupportedException(FieldType.DataType);
    }
  }

  public override bool Equals(object? obj) => obj is FieldIndexEntity index && index.FieldIndexId == FieldIndexId;
  public override int GetHashCode() => FieldIndexId.GetHashCode();
  public override string ToString() => $"{GetType()} (FieldIndexId={FieldIndexId})";
}
