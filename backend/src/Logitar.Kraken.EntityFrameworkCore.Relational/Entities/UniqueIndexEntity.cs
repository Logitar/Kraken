using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class UniqueIndexEntity : ISegregatedEntity
{
  public const char KeySeparator = '|';
  public const int MaximumLength = byte.MaxValue;

  public int UniqueIndexId { get; private set; }

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

  public long Revision { get; private set; }
  public ContentStatus Status { get; private set; }

  public string Value { get; private set; } = string.Empty;
  public string ValueNormalized
  {
    get => Helper.Normalize(Value);
    private set { }
  }

  public string Key
  {
    get => CreateKey(FieldDefinitionUid, ValueNormalized);
    private set { }
  }

  public ContentEntity? Content { get; private set; }
  public int ContentId { get; private set; }
  public Guid ContentUid { get; private set; }

  public ContentLocaleEntity? ContentLocale { get; private set; }
  public int ContentLocaleId { get; private set; }
  public string ContentLocaleName { get; private set; } = string.Empty;

  public UniqueIndexEntity(
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

    Status = status;

    long revision = (status == ContentStatus.Published ? contentLocale.PublishedRevision : null) ?? contentLocale.Revision;
    Update(revision, value);

    Content = content;
    ContentId = content.ContentId;
    ContentUid = content.Id;

    ContentLocale = contentLocale;
    ContentLocaleId = contentLocale.ContentLocaleId;
    ContentLocaleName = contentLocale.UniqueNameNormalized;
  }

  private UniqueIndexEntity()
  {
  }

  public static string CreateKey(KeyValuePair<Guid, string> fieldValue) => CreateKey(fieldValue.Key, fieldValue.Value);
  public static string CreateKey(Guid fieldDefinitionId, string value) => string.Join(KeySeparator,
    Convert.ToBase64String(fieldDefinitionId.ToByteArray()).TrimEnd('='),
    Helper.Normalize(value));

  public void Update(long revision, string value)
  {
    Revision = revision;

    Value = value.Truncate(MaximumLength);
  }

  public override bool Equals(object? obj) => obj is UniqueIndexEntity index && index.UniqueIndexId == UniqueIndexId;
  public override int GetHashCode() => UniqueIndexId.GetHashCode();
  public override string ToString() => $"{GetType()} (UniqueIndexId={UniqueIndexId})";
}
