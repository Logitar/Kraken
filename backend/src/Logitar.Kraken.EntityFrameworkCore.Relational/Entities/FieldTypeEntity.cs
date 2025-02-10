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
  public string? Properties { get; private set; }

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
    if (Properties == null || DataType != DataType.Boolean)
    {
      return null;
    }
    return JsonSerializer.Deserialize<BooleanPropertiesModel>(Properties);
  }
  public void SetProperties(FieldTypeBooleanPropertiesChanged @event)
  {
    Update(@event);

    BooleanPropertiesModel properties = new(@event.Properties);
    Properties = JsonSerializer.Serialize(properties);
  }

  public DateTimePropertiesModel? GetDateTimeProperties()
  {
    if (Properties == null || DataType != DataType.DateTime)
    {
      return null;
    }
    return JsonSerializer.Deserialize<DateTimePropertiesModel>(Properties);
  }
  public void SetProperties(FieldTypeDateTimePropertiesChanged @event)
  {
    Update(@event);

    DateTimePropertiesModel properties = new(@event.Properties);
    Properties = JsonSerializer.Serialize(properties);
  }

  public NumberPropertiesModel? GetNumberProperties()
  {
    if (Properties == null || DataType != DataType.Number)
    {
      return null;
    }
    return JsonSerializer.Deserialize<NumberPropertiesModel>(Properties);
  }
  public void SetProperties(FieldTypeNumberPropertiesChanged @event)
  {
    Update(@event);

    NumberPropertiesModel properties = new(@event.Properties);
    Properties = JsonSerializer.Serialize(properties);
  }

  public RelatedContentPropertiesModel? GetRelatedContentProperties()
  {
    if (Properties == null || DataType != DataType.RelatedContent)
    {
      return null;
    }
    return JsonSerializer.Deserialize<RelatedContentPropertiesModel>(Properties);
  }
  public void SetProperties(FieldTypeRelatedContentPropertiesChanged @event)
  {
    Update(@event);

    RelatedContentPropertiesModel properties = new()
    {
      ContentTypeId = @event.Properties.ContentTypeId.EntityId,
      IsMultiple = @event.Properties.IsMultiple
    };
    Properties = JsonSerializer.Serialize(properties);
  }

  public RichTextPropertiesModel? GetRichTextProperties()
  {
    if (Properties == null || DataType != DataType.RichText)
    {
      return null;
    }
    return JsonSerializer.Deserialize<RichTextPropertiesModel>(Properties);
  }
  public void SetProperties(FieldTypeRichTextPropertiesChanged @event)
  {
    Update(@event);

    RichTextPropertiesModel properties = new(@event.Properties);
    Properties = JsonSerializer.Serialize(properties);
  }

  public SelectPropertiesModel? GetSelectProperties()
  {
    if (Properties == null || DataType != DataType.Select)
    {
      return null;
    }
    return JsonSerializer.Deserialize<SelectPropertiesModel>(Properties);
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
    Properties = JsonSerializer.Serialize(properties);
  }

  public StringPropertiesModel? GetStringProperties()
  {
    if (Properties == null || DataType != DataType.String)
    {
      return null;
    }
    return JsonSerializer.Deserialize<StringPropertiesModel>(Properties);
  }
  public void SetProperties(FieldTypeStringPropertiesChanged @event)
  {
    Update(@event);

    StringPropertiesModel properties = new(@event.Properties);
    Properties = JsonSerializer.Serialize(properties);
  }

  public TagsPropertiesModel? GetTagsProperties()
  {
    if (Properties == null || DataType != DataType.Tags)
    {
      return null;
    }
    return JsonSerializer.Deserialize<TagsPropertiesModel>(Properties);
  }
  public void SetProperties(FieldTypeTagsPropertiesChanged @event)
  {
    Update(@event);

    TagsPropertiesModel properties = new(@event.Properties);
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
