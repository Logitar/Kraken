using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class ContentLocaleEntity
{
  public int ContentLocaleId { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }

  public ContentEntity? Content { get; private set; }
  public int ContentId { get; private set; }

  public LanguageEntity? Language { get; private set; }
  public int? LanguageId { get; private set; }

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

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }

  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public bool IsPublished { get; private set; }
  public long? PublishedRevision { get; private set; }
  public string? PublishedBy { get; private set; }
  public DateTime? PublishedOn { get; private set; }

  public PublishedContentEntity? PublishedContent { get; private set; }
  public List<FieldIndexEntity> FieldIndex { get; private set; } = [];
  public List<UniqueIndexEntity> UniqueIndex { get; private set; } = [];

  public ContentLocaleEntity(ContentEntity content, ContentCreated @event)
    : this(content, language: null, @event.Invariant, @event)
  {
  }
  public ContentLocaleEntity(ContentEntity content, LanguageEntity? language, ContentLocaleChanged @event)
    : this(content, language, @event.Locale, @event)
  {
  }
  private ContentLocaleEntity(ContentEntity content, LanguageEntity? language, ContentLocale locale, DomainEvent @event)
  {
    Revision = 1;

    ContentType = content.ContentType;
    ContentTypeId = content.ContentTypeId;

    Content = content;
    ContentId = content.ContentId;

    Language = language;
    LanguageId = language?.LanguageId;

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(locale, @event);
  }

  private ContentLocaleEntity()
  {
  }

  public IReadOnlyCollection<ActorId> GetActorIds(bool includeContent = true)
  {
    List<ActorId> actorIds = [];
    if (CreatedBy != null)
    {
      actorIds.Add(new ActorId(CreatedBy));
    }
    if (UpdatedBy != null)
    {
      actorIds.Add(new ActorId(UpdatedBy));
    }
    if (PublishedBy != null)
    {
      actorIds.Add(new ActorId(PublishedBy));
    }
    if (ContentType != null)
    {
      actorIds.AddRange(ContentType.GetActorIds());
    }
    if (includeContent && Content != null)
    {
      actorIds.AddRange(Content.GetActorIds());
    }
    if (Language != null)
    {
      actorIds.AddRange(Language.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public void Publish(ContentLocalePublished @event)
  {
    if (PublishedContent == null)
    {
      PublishedContent = new(this, @event);
    }
    else
    {
      PublishedContent.Update(this, @event);
    }

    IsPublished = true;
    PublishedRevision = Revision;
    PublishedBy = @event.ActorId?.Value;
    PublishedOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Unpublish(ContentLocaleUnpublished _)
  {
    IsPublished = false;
    PublishedRevision = null;
    PublishedBy = null;
    PublishedOn = null;
  }

  public void Update(ContentLocale locale, DomainEvent @event)
  {
    Revision++;

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    UpdatedBy = @event.ActorId?.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();

    SetFieldValues(locale.FieldValues);
  }

  public Dictionary<Guid, string> GetFieldValues()
  {
    return (FieldValues == null ? null : JsonSerializer.Deserialize<Dictionary<Guid, string>>(FieldValues)) ?? [];
  }
  private void SetFieldValues(IReadOnlyDictionary<Guid, string> fieldValues)
  {
    FieldValues = fieldValues.Count < 1 ? null : JsonSerializer.Serialize(fieldValues);
  }

  public override bool Equals(object? obj) => obj is ContentLocaleEntity entity && entity.ContentLocaleId == ContentLocaleId;
  public override int GetHashCode() => ContentLocaleId.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (ContentLocaleId={ContentLocaleId})";
}
