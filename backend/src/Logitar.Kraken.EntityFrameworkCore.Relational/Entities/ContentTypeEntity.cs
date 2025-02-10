using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class ContentTypeEntity : AggregateEntity, ISegregatedEntity
{
  public int ContentTypeId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public bool IsInvariant { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public int FieldCount { get; private set; }

  public List<ContentEntity> Contents { get; private set; } = [];
  public List<ContentLocaleEntity> ContentLocales { get; private set; } = [];
  public List<FieldDefinitionEntity> Fields { get; private set; } = [];
  public List<FieldIndexEntity> FieldIndex { get; private set; } = [];
  public List<PublishedContentEntity> PublishedContents { get; private set; } = [];
  public List<UniqueIndexEntity> UniqueIndex { get; private set; } = [];

  public ContentTypeEntity(RealmEntity? realm, ContentTypeCreated @event) : base(@event)
  {
    if (realm != null)
    {
      Realm = realm;
      RealmId = realm.RealmId;
      RealmUid = realm.Id;
    }

    ContentTypeId contentTypeId = new(@event.StreamId);
    Id = contentTypeId.EntityId;

    IsInvariant = @event.IsInvariant;

    UniqueName = @event.UniqueName.Value;
  }

  private ContentTypeEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = [.. base.GetActorIds()];
    foreach (FieldDefinitionEntity field in Fields)
    {
      if (field.FieldType != null)
      {
        actorIds.AddRange(field.FieldType.GetActorIds());
      }
    }
    return actorIds.AsReadOnly();
  }

  public void SetField(FieldTypeEntity fieldType, ContentTypeFieldDefinitionChanged @event)
  {
    Update(@event);

    FieldDefinitionEntity? fieldDefinition = Fields.SingleOrDefault(x => x.Id == @event.FieldDefinition.Id);
    if (fieldDefinition == null)
    {
      fieldDefinition = new(this, fieldType, order: FieldCount, @event);
      Fields.Add(fieldDefinition);
      FieldCount = Fields.Count;
    }
    else
    {
      fieldDefinition.Update(@event);
    }
  }

  public void SetUniqueName(ContentTypeUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(ContentTypeUpdated @event)
  {
    base.Update(@event);

    if (@event.IsInvariant.HasValue)
    {
      IsInvariant = @event.IsInvariant.Value;
    }

    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
