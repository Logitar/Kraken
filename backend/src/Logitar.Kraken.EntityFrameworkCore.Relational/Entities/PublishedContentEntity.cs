using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class PublishedContentEntity
{
  public ContentLocaleEntity? ContentLocale { get; private set; }
  public int ContentLocaleId { get; private set; }

  public ContentEntity? Content { get; private set; }
  public int ContentId { get; private set; }
  public Guid ContentUid { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public Guid ContentTypeUid { get; private set; }
  public string ContentTypeName { get; private set; } = string.Empty;

  public LanguageEntity? Language { get; private set; }
  public int? LanguageId { get; private set; }
  public Guid? LanguageUid { get; private set; }
  public string? LanguageCode { get; private set; }
  public bool LanguageIsDefault { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string? FieldValues { get; private set; }

  public long Revision { get; private set; }
  public string? PublishedBy { get; private set; }
  public DateTime PublishedOn { get; private set; }

  public PublishedContentEntity(ContentLocaleEntity contentLocale, ContentLocalePublished @event)
  {
    ContentLocale = contentLocale;
    ContentLocaleId = contentLocale.ContentLocaleId;

    ContentEntity content = contentLocale.Content ?? throw new ArgumentException("The content is required.", nameof(contentLocale));
    Content = content;
    ContentId = content.ContentId;
    ContentUid = content.Id;

    ContentTypeEntity contentType = content.ContentType ?? throw new ArgumentException("The content type is required.", nameof(contentLocale));
    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;
    ContentTypeUid = contentType.Id;
    ContentTypeName = contentType.UniqueNameNormalized;

    if (contentLocale.LanguageId.HasValue)
    {
      LanguageEntity language = contentLocale.Language ?? throw new ArgumentException("The language is required.", nameof(contentLocale));
      Language = language;
      LanguageId = language.LanguageId;
      LanguageUid = language.Id;
      LanguageCode = language.CodeNormalized;
      LanguageIsDefault = language.IsDefault;
    }

    Update(contentLocale, @event);
  }

  private PublishedContentEntity()
  {
  }

  public IReadOnlyCollection<ActorId> GetActorIds() => PublishedBy == null ? [] : [new ActorId(PublishedBy)];

  public Dictionary<Guid, string> GetFieldValues()
  {
    return (FieldValues == null ? null : JsonSerializer.Deserialize<Dictionary<Guid, string>>(FieldValues)) ?? [];
  }

  public void Update(ContentLocaleEntity contentLocale, ContentLocalePublished @event)
  {
    UniqueName = contentLocale.UniqueName;
    DisplayName = contentLocale.DisplayName;
    Description = contentLocale.Description;
    FieldValues = contentLocale.FieldValues;

    Revision = contentLocale.Revision;
    PublishedBy = @event.ActorId?.Value;
    PublishedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is PublishedContentEntity publishedContent && publishedContent.ContentLocaleId == ContentLocaleId;
  public override int GetHashCode() => ContentLocaleId.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (ContentLocaleId={ContentLocaleId})";
}
