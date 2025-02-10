using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Contents.Events;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class ContentEntity : AggregateEntity, ISegregatedEntity
{
  public int ContentId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }

  public List<FieldIndexEntity> FieldIndex { get; private set; } = [];
  public List<ContentLocaleEntity> Locales { get; private set; } = [];
  public List<PublishedContentEntity> PublishedContents { get; private set; } = [];
  public List<UniqueIndexEntity> UniqueIndex { get; private set; } = [];

  public ContentEntity(ContentTypeEntity contentType, ContentCreated @event) : base(@event)
  {
    Realm = contentType.Realm;
    RealmId = contentType.RealmId;
    RealmUid = contentType.RealmUid;

    ContentId contentId = new(@event.StreamId);
    Id = contentId.EntityId;

    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;

    ContentLocaleEntity invariant = new(this, @event);
    Locales.Add(invariant);
  }

  private ContentEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(includeLocales: true);
  public IReadOnlyCollection<ActorId> GetActorIds(bool includeLocales)
  {
    List<ActorId> actorIds = [.. base.GetActorIds()];
    if (ContentType != null)
    {
      actorIds.AddRange(ContentType.GetActorIds());
    }
    if (includeLocales)
    {
      foreach (ContentLocaleEntity locale in Locales)
      {
        actorIds.AddRange(locale.GetActorIds(includeContent: false));
      }
    }
    return actorIds.AsReadOnly();
  }

  public ContentLocaleEntity? Publish(ContentLocalePublished @event)
  {
    Update(@event);

    ContentLocaleEntity? locale = Locales.SingleOrDefault(l => @event.LanguageId.HasValue
      ? (l.Language != null && l.Language.Id == @event.LanguageId.Value.EntityId)
      : (l.Language == null));
    if (locale == null)
    {
      return null;
    }

    locale.Publish(@event);
    return locale;
  }

  public void SetLocale(LanguageEntity? language, ContentLocaleChanged @event)
  {
    Update(@event);

    ContentLocaleEntity? locale = Locales.SingleOrDefault(x => x.LanguageId == language?.LanguageId);
    if (locale == null)
    {
      locale = new(this, language, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event.Locale, @event);
    }
  }

  public ContentLocaleEntity? Unpublish(ContentLocaleUnpublished @event)
  {
    Update(@event);

    ContentLocaleEntity? locale = Locales.SingleOrDefault(l => @event.LanguageId.HasValue
      ? (l.Language != null && l.Language.Id == @event.LanguageId.Value.EntityId)
      : (l.Language == null));
    if (locale == null)
    {
      return null;
    }

    locale.Unpublish(@event);
    return locale;
  }
}
