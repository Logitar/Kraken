using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Fields.Events;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class FieldTypeEntity : AggregateEntity, ISegregatedEntity
{
  public int FieldTypeId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

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
  public string? Settings { get; private set; }

  public List<FieldDefinitionEntity> FieldDefinitions { get; private set; } = [];
  public List<FieldIndexEntity> FieldIndex { get; private set; } = [];
  public List<UniqueIndexEntity> UniqueIndex { get; private set; } = [];

  public FieldTypeEntity(RealmEntity? realm, FieldTypeCreated @event) : base(@event)
  {
    if (realm != null)
    {
      Realm = realm;
      RealmId = realm.RealmId;
      RealmUid = realm.Id;
    }

    FieldTypeId fieldTypeId = new(@event.StreamId);
    Id = fieldTypeId.EntityId;

    UniqueName = @event.UniqueName.Value;

    DataType = @event.DataType;
  }

  private FieldTypeEntity() : base()
  {
  }

  public BooleanPropertiesModel? GetBooleanProperties()
  {
    if (Settings == null || DataType != DataType.Boolean)
    {
      return null;
    }
    return JsonSerializer.Deserialize<BooleanPropertiesModel>(Settings);
  }
  public void SetProperties(FieldTypeBooleanPropertiesChanged @event)
  {
    Update(@event);

    BooleanPropertiesModel properties = new(@event.Properties);
    Settings = JsonSerializer.Serialize(properties);
  }

  public DateTimePropertiesModel? GetDateTimeProperties()
  {
    if (Settings == null || DataType != DataType.DateTime)
    {
      return null;
    }
    return JsonSerializer.Deserialize<DateTimePropertiesModel>(Settings);
  }
  public void SetProperties(FieldTypeDateTimePropertiesChanged @event)
  {
    Update(@event);

    DateTimePropertiesModel properties = new(@event.Properties);
    Settings = JsonSerializer.Serialize(properties);
  }

  public NumberPropertiesModel? GetNumberProperties()
  {
    if (Settings == null || DataType != DataType.Number)
    {
      return null;
    }
    return JsonSerializer.Deserialize<NumberPropertiesModel>(Settings);
  }
  public void SetProperties(FieldTypeNumberPropertiesChanged @event)
  {
    Update(@event);

    NumberPropertiesModel properties = new(@event.Properties);
    Settings = JsonSerializer.Serialize(properties);
  }

  public RelatedContentPropertiesModel? GetRelatedContentProperties()
  {
    if (Settings == null || DataType != DataType.RelatedContent)
    {
      return null;
    }
    return JsonSerializer.Deserialize<RelatedContentPropertiesModel>(Settings);
  }
  public void SetProperties(FieldTypeRelatedContentPropertiesChanged @event)
  {
    Update(@event);

    RelatedContentPropertiesModel properties = new()
    {
      ContentTypeId = @event.Properties.ContentTypeId.EntityId,
      IsMultiple = @event.Properties.IsMultiple
    };
    Settings = JsonSerializer.Serialize(properties);
  }

  public RichTextPropertiesModel? GetRichTextProperties()
  {
    if (Settings == null || DataType != DataType.RichText)
    {
      return null;
    }
    return JsonSerializer.Deserialize<RichTextPropertiesModel>(Settings);
  }
  public void SetProperties(FieldTypeRichTextPropertiesChanged @event)
  {
    Update(@event);

    RichTextPropertiesModel properties = new(@event.Properties);
    Settings = JsonSerializer.Serialize(properties);
  }

  public SelectPropertiesModel? GetSelectProperties()
  {
    if (Settings == null || DataType != DataType.Select)
    {
      return null;
    }
    return JsonSerializer.Deserialize<SelectPropertiesModel>(Settings);
  }
  public void SetProperties(FieldTypeSelectPropertiesChanged @event)
  {
    Update(@event);

    SelectPropertiesModel properties = new()
    {
      IsMultiple = @event.Properties.IsMultiple
    };
    foreach (SelectOption option in @event.Properties.Options)
    {
      properties.Options.Add(new SelectOptionModel(option));
    }
    Settings = JsonSerializer.Serialize(properties);
  }

  public StringPropertiesModel? GetStringProperties()
  {
    if (Settings == null || DataType != DataType.String)
    {
      return null;
    }
    return JsonSerializer.Deserialize<StringPropertiesModel>(Settings);
  }
  public void SetProperties(FieldTypeStringPropertiesChanged @event)
  {
    Update(@event);

    StringPropertiesModel properties = new(@event.Properties);
    Settings = JsonSerializer.Serialize(properties);
  }

  public TagsPropertiesModel? GetTagsProperties()
  {
    if (Settings == null || DataType != DataType.Tags)
    {
      return null;
    }
    return JsonSerializer.Deserialize<TagsPropertiesModel>(Settings);
  }
  public void SetProperties(FieldTypeTagsPropertiesChanged @event)
  {
    Update(@event);

    TagsPropertiesModel properties = new(@event.Properties);
    Settings = JsonSerializer.Serialize(properties);
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
