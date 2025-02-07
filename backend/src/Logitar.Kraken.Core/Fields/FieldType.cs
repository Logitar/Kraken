using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Fields.Events;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;

namespace Logitar.Kraken.Core.Fields;

public class FieldType : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new UniqueNameSettings();

  private FieldTypeUpdated _updated = new();

  public new FieldTypeId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The field type has not been initialized.");
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (_displayName != value)
      {
        _displayName = value;
        _updated.DisplayName = new Change<DisplayName>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Change<Description>(value);
      }
    }
  }

  public DataType DataType { get; private set; }
  private FieldTypeProperties? _properties = null;
  public FieldTypeProperties Properties => _properties ?? throw new InvalidOperationException("The field type has not been initialized.");

  public FieldType() : base()
  {
  }

  public FieldType(UniqueName uniqueName, FieldTypeProperties properties, ActorId? actorId = null, FieldTypeId? fieldTypeId = null) : base(fieldTypeId?.StreamId)
  {
    Raise(new FieldTypeCreated(uniqueName, properties.DataType), actorId);
    switch (properties.DataType)
    {
      case DataType.Boolean:
        SetProperties((BooleanProperties)properties, actorId);
        break;
      case DataType.DateTime:
        SetProperties((DateTimeProperties)properties, actorId);
        break;
      case DataType.Number:
        SetProperties((NumberProperties)properties, actorId);
        break;
      case DataType.RelatedContent:
        SetProperties((RelatedContentProperties)properties, actorId);
        break;
      case DataType.RichText:
        SetProperties((RichTextProperties)properties, actorId);
        break;
      case DataType.Select:
        SetProperties((SelectProperties)properties, actorId);
        break;
      case DataType.String:
        SetProperties((StringProperties)properties, actorId);
        break;
      case DataType.Tags:
        SetProperties((TagsProperties)properties, actorId);
        break;
      default:
        throw new DataTypeNotSupportedException(properties.DataType);
    }
  }
  protected virtual void Handle(FieldTypeCreated @event)
  {
    _uniqueName = @event.UniqueName;

    DataType = @event.DataType;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new FieldTypeDeleted(), actorId);
    }
  }

  public void SetProperties(BooleanProperties properties, ActorId? actorId = null)
  {
    if (DataType != properties.DataType)
    {
      throw new UnexpectedFieldTypePropertiesException(this, properties);
    }

    if (_properties == null || !_properties.Equals(properties))
    {
      Raise(new FieldTypeBooleanPropertiesChanged(properties), actorId);
    }
  }
  protected virtual void Handle(FieldTypeBooleanPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(DateTimeProperties properties, ActorId? actorId = null)
  {
    if (DataType != properties.DataType)
    {
      throw new UnexpectedFieldTypePropertiesException(this, properties);
    }

    if (_properties == null || !_properties.Equals(properties))
    {
      Raise(new FieldTypeDateTimePropertiesChanged(properties), actorId);
    }
  }
  protected virtual void Handle(FieldTypeDateTimePropertiesChanged @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(NumberProperties properties, ActorId? actorId = null)
  {
    if (DataType != properties.DataType)
    {
      throw new UnexpectedFieldTypePropertiesException(this, properties);
    }

    if (_properties == null || !_properties.Equals(properties))
    {
      Raise(new FieldTypeNumberPropertiesChanged(properties), actorId);
    }
  }
  protected virtual void Handle(FieldTypeNumberPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(RelatedContentProperties properties, ActorId? actorId = null)
  {
    if (DataType != properties.DataType)
    {
      throw new UnexpectedFieldTypePropertiesException(this, properties);
    }

    if (_properties == null || !_properties.Equals(properties))
    {
      Raise(new FieldTypeRelatedContentPropertiesChanged(properties), actorId);
    }
  }
  protected virtual void Handle(FieldTypeRelatedContentPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(RichTextProperties properties, ActorId? actorId = null)
  {
    if (DataType != properties.DataType)
    {
      throw new UnexpectedFieldTypePropertiesException(this, properties);
    }

    if (_properties == null || !_properties.Equals(properties))
    {
      Raise(new FieldTypeRichTextPropertiesChanged(properties), actorId);
    }
  }
  protected virtual void Handle(FieldTypeRichTextPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(SelectProperties properties, ActorId? actorId = null)
  {
    if (DataType != properties.DataType)
    {
      throw new UnexpectedFieldTypePropertiesException(this, properties);
    }

    if (_properties == null || !_properties.Equals(properties))
    {
      Raise(new FieldTypeSelectPropertiesChanged(properties), actorId);
    }
  }
  protected virtual void Handle(FieldTypeSelectPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(StringProperties properties, ActorId? actorId = null)
  {
    if (DataType != properties.DataType)
    {
      throw new UnexpectedFieldTypePropertiesException(this, properties);
    }

    if (_properties == null || !_properties.Equals(properties))
    {
      Raise(new FieldTypeStringPropertiesChanged(properties), actorId);
    }
  }
  protected virtual void Handle(FieldTypeStringPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }

  public void SetProperties(TagsProperties properties, ActorId? actorId = null)
  {
    if (DataType != properties.DataType)
    {
      throw new UnexpectedFieldTypePropertiesException(this, properties);
    }

    if (_properties == null || !_properties.Equals(properties))
    {
      Raise(new FieldTypeTagsPropertiesChanged(properties), actorId);
    }
  }
  protected virtual void Handle(FieldTypeTagsPropertiesChanged @event)
  {
    _properties = @event.Properties;
  }

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new FieldTypeUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(FieldTypeUniqueNameChanged @event)
  {
    _uniqueName = @event.UniqueName;
  }

  public void Update(ActorId? actorId = null)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(FieldTypeUpdated updated)
  {
    if (updated.DisplayName != null)
    {
      _displayName = updated.DisplayName.Value;
    }
    if (updated.Description != null)
    {
      _description = updated.Description.Value;
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}
