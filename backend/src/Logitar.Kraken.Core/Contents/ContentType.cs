using FluentValidation;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Contents;

public class ContentType : AggregateRoot
{
  private ContentTypeUpdated _updated = new();

  public new ContentTypeId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private bool _isInvariant = false;
  public bool IsInvariant
  {
    get => _isInvariant;
    set
    {
      if (_isInvariant != value)
      {
        _isInvariant = value;
        _updated.IsInvariant = value;
      }
    }
  }

  private Identifier? _uniqueName = null;
  public Identifier UniqueName => _uniqueName ?? throw new InvalidOperationException("The content type has not been initialized.");
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

  private readonly Dictionary<Guid, int> _fieldsById = [];
  private readonly Dictionary<Identifier, int> _fieldsByUniqueName = [];
  private readonly List<FieldDefinition> _fieldDefinitions = [];
  public IReadOnlyCollection<FieldDefinition> FieldDefinitions => _fieldDefinitions.AsReadOnly();

  public ContentType() : base()
  {
  }

  public ContentType(Identifier uniqueName, bool isInvariant = true, ActorId? actorId = null, ContentTypeId? contentTypeId = null) : base(contentTypeId?.StreamId)
  {
    Raise(new ContentTypeCreated(isInvariant, uniqueName), actorId);
  }
  protected virtual void Handle(ContentTypeCreated @event)
  {
    _isInvariant = @event.IsInvariant;

    _uniqueName = @event.UniqueName;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new ContentTypeDeleted(), actorId);
    }
  }

  public FieldDefinition FindField(Guid id) => TryGetField(id) ?? throw new InvalidOperationException($"The field 'Id={id}' could not be found.");
  public FieldDefinition FindField(Identifier uniqueName) => TryGetField(uniqueName) ?? throw new InvalidOperationException($"The field 'UniqueName={uniqueName}' could not be found.");

  public void SetField(FieldDefinition fieldDefinition, ActorId? actorId = null)
  {
    if (IsInvariant && !fieldDefinition.IsInvariant)
    {
      ValidationFailure failure = new(nameof(fieldDefinition.IsInvariant), "'IsInvariant' must be true. Invariant content types cannot define variant fields.", fieldDefinition.IsInvariant)
      {
        ErrorCode = "InvariantValidator"
      };
      throw new ValidationException([failure]);
    }

    if (!_fieldsById.TryGetValue(fieldDefinition.Id, out int index))
    {
      index = -1;
    }
    if (_fieldsByUniqueName.TryGetValue(fieldDefinition.UniqueName, out int conflict) && conflict != index)
    {
      Guid conflictId = _fieldsById.Where(x => x.Value == conflict).Single().Key;
      throw new UniqueNameAlreadyUsedException(RealmId?.ToGuid(), conflictId, fieldDefinition.Id, fieldDefinition.UniqueName.Value, nameof(fieldDefinition.UniqueName));
    }

    FieldDefinition? existingField = index < 0 ? null : _fieldDefinitions.ElementAt(index);
    if (existingField == null || !existingField.Equals(fieldDefinition))
    {
      Raise(new ContentTypeFieldDefinitionChanged(fieldDefinition), actorId);
    }
  }
  protected virtual void Handle(ContentTypeFieldDefinitionChanged @event)
  {
    if (_fieldsById.TryGetValue(@event.FieldDefinition.Id, out int index))
    {
      FieldDefinition existingField = _fieldDefinitions.ElementAt(index);
      _fieldDefinitions[index] = @event.FieldDefinition;

      if (!existingField.UniqueName.Equals(@event.FieldDefinition.UniqueName))
      {
        _fieldsByUniqueName.Remove(existingField.UniqueName);
        _fieldsByUniqueName[@event.FieldDefinition.UniqueName] = index;
      }
    }
    else
    {
      index = _fieldDefinitions.Count;

      _fieldDefinitions.Add(@event.FieldDefinition);

      _fieldsById[@event.FieldDefinition.Id] = index;
      _fieldsByUniqueName[@event.FieldDefinition.UniqueName] = index;
    }
  }

  public void SetUniqueName(Identifier uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new ContentTypeUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(ContentTypeUniqueNameChanged @event)
  {
    _uniqueName = @event.UniqueName;
  }

  public FieldDefinition? TryGetField(Guid id) => _fieldsById.TryGetValue(id, out int index) ? _fieldDefinitions.ElementAt(index) : null;
  public FieldDefinition? TryGetField(Identifier uniqueName) => _fieldsByUniqueName.TryGetValue(uniqueName, out int index) ? _fieldDefinitions.ElementAt(index) : null;

  public void Update(ActorId? actorId = null)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(ContentTypeUpdated updated)
  {
    if (updated.IsInvariant.HasValue)
    {
      _isInvariant = updated.IsInvariant.Value;
    }

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
