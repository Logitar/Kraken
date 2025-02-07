using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Fields.Events;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class FieldTypeEntity : AggregateEntity
{
  public int FieldTypeId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public Guid Id { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public DataType DataType { get; private set; }
  public string? Properties { get; private set; }

  public List<FieldDefinitionEntity> FieldDefinitions { get; private set; } = [];
  //public List<FieldIndexEntity> FieldIndex { get; private set; } = [];
  //public List<UniqueIndexEntity> UniqueIndex { get; private set; } = [];

  public FieldTypeEntity(RealmEntity? realm, FieldTypeCreated @event) : base(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;

    FieldTypeId fieldTypeId = new(@event.StreamId);
    Id = fieldTypeId.EntityId;

    UniqueName = @event.UniqueName.Value;

    DataType = @event.DataType;
  }

  private FieldTypeEntity() : base()
  {
  }

  public void SetProperties(FieldTypeBooleanPropertiesChanged @event)
  {
    SetProperties(@event.Properties, @event);
  }
  public void SetProperties(FieldTypeDateTimePropertiesChanged @event)
  {
    SetProperties(@event.Properties, @event);
  }
  public void SetProperties(FieldTypeNumberPropertiesChanged @event)
  {
    SetProperties(@event.Properties, @event);
  }
  public void SetProperties(FieldTypeRelatedContentPropertiesChanged @event)
  {
    RelatedContentPropertiesModel properties = new()
    {
      ContentTypeId = @event.Properties.ContentTypeId.EntityId,
      IsMultiple = @event.Properties.IsMultiple
    };
    SetProperties(properties, @event);
  }
  public void SetProperties(FieldTypeRichTextPropertiesChanged @event)
  {
    SetProperties(@event.Properties, @event);
  }
  public void SetProperties(FieldTypeSelectPropertiesChanged @event)
  {
    SelectPropertiesModel properties = new()
    {
      IsMultiple = @event.Properties.IsMultiple
    };
    foreach (SelectOption option in @event.Properties.Options)
    {
      properties.Options.Add(new SelectOptionModel(option));
    }
    SetProperties(properties, @event);
  }
  public void SetProperties(FieldTypeStringPropertiesChanged @event)
  {
    SetProperties(@event.Properties, @event);
  }
  public void SetProperties(FieldTypeTagsPropertiesChanged @event)
  {
    SetProperties(@event.Properties, @event);
  }
  private void SetProperties<T>(T properties, DomainEvent @event)
  {
    Update(@event);

    Properties = JsonSerializer.Serialize(properties);
  }

  public void SetUniqueName(FieldTypeUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(FieldTypeUpdated @event)
  {
    base.Update(@event);

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
