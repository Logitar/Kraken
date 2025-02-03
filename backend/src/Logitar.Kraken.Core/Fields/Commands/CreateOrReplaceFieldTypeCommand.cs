using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Kraken.Core.Fields.Validators;
using Logitar.Kraken.Core.Realms;
using MediatR;

namespace Logitar.Kraken.Core.Fields.Commands;

public record CreateOrReplaceFieldTypeResult(FieldTypeModel? FieldType = null, bool Created = false);

/// <exception cref="ArgumentException"></exception>
/// <exception cref="ContentTypeNotFoundException"></exception>
/// <exception cref="DataTypeNotSupportedException"></exception>
/// <exception cref="UnexpectedFieldTypePropertiesException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceFieldTypeCommand(Guid? Id, CreateOrReplaceFieldTypePayload Payload, long? Version) : IRequest<CreateOrReplaceFieldTypeResult>;

internal class CreateOrReplaceFieldTypeCommandHandler : IRequestHandler<CreateOrReplaceFieldTypeCommand, CreateOrReplaceFieldTypeResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IFieldTypeManager _fieldTypeManager;
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public CreateOrReplaceFieldTypeCommandHandler(
    IApplicationContext applicationContext,
    IFieldTypeManager fieldTypeManager,
    IFieldTypeQuerier fieldTypeQuerier,
    IFieldTypeRepository fieldTypeRepository)
  {
    _applicationContext = applicationContext;
    _fieldTypeManager = fieldTypeManager;
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task<CreateOrReplaceFieldTypeResult> Handle(CreateOrReplaceFieldTypeCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldTypePayload payload = command.Payload;
    new CreateOrReplaceFieldTypeValidator().ValidateAndThrow(payload);

    UniqueName uniqueName = new(FieldType.UniqueNameSettings, payload.UniqueName);
    ActorId? actorId = _applicationContext.ActorId;

    RealmId? realmId = _applicationContext.RealmId;
    FieldTypeId fieldTypeId = FieldTypeId.NewId(realmId);
    FieldType? fieldType = null;
    if (command.Id.HasValue)
    {
      fieldTypeId = new(realmId, command.Id.Value);
      fieldType = await _fieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken);
    }

    bool created = false;
    if (fieldType == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceFieldTypeResult();
      }

      FieldTypeProperties settings = GetProperties(payload, realmId);
      fieldType = new(uniqueName, settings, actorId, fieldTypeId);
      created = true;
    }

    FieldType reference = (command.Version.HasValue
      ? await _fieldTypeRepository.LoadAsync(fieldType.Id, command.Version.Value, cancellationToken)
      : null) ?? fieldType;

    if (!created)
    {
      if (reference.UniqueName != uniqueName)
      {
        fieldType.SetUniqueName(uniqueName, actorId);
      }

      SetProperties(payload, fieldType, reference, actorId);
    }

    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      fieldType.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      fieldType.Description = description;
    }

    fieldType.Update(actorId);

    await _fieldTypeManager.SaveAsync(fieldType, cancellationToken);

    FieldTypeModel model = await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
    return new CreateOrReplaceFieldTypeResult(model, created);
  }

  private static FieldTypeProperties GetProperties(CreateOrReplaceFieldTypePayload payload, RealmId? realmId)
  {
    List<FieldTypeProperties> settings = new(capacity: 8);

    if (payload.Boolean != null)
    {
      settings.Add(new BooleanProperties(payload.Boolean));
    }
    if (payload.DateTime != null)
    {
      settings.Add(new DateTimeProperties(payload.DateTime));
    }
    if (payload.Number != null)
    {
      settings.Add(new NumberProperties(payload.Number));
    }
    if (payload.RelatedContent != null)
    {
      settings.Add(payload.RelatedContent.ToRelatedContentProperties(realmId));
    }
    if (payload.RichText != null)
    {
      settings.Add(new RichTextProperties(payload.RichText));
    }
    if (payload.Select != null)
    {
      settings.Add(payload.Select.ToSelectProperties());
    }
    if (payload.String != null)
    {
      settings.Add(new StringProperties(payload.String));
    }
    if (payload.Tags != null)
    {
      settings.Add(new TagsProperties(payload.Tags));
    }

    if (settings.Count < 1)
    {
      throw new ArgumentException("The field type payload did not provide any settings.", nameof(payload));
    }
    else if (settings.Count > 1)
    {
      throw new ArgumentException($"The field type payload provided {settings.Count} settings.", nameof(payload));
    }
    return settings.Single();
  }

  private static void SetProperties(CreateOrReplaceFieldTypePayload payload, FieldType fieldType, FieldType reference, ActorId? actorId)
  {
    if (payload.Boolean != null)
    {
      BooleanProperties properties = new(payload.Boolean);
      if (!reference.Properties.Equals(properties))
      {
        fieldType.SetProperties(properties, actorId);
      }
    }
    if (payload.DateTime != null)
    {
      DateTimeProperties properties = new(payload.DateTime);
      if (!reference.Properties.Equals(properties))
      {
        fieldType.SetProperties(properties, actorId);
      }
    }
    if (payload.Number != null)
    {
      NumberProperties properties = new(payload.Number);
      if (!reference.Properties.Equals(properties))
      {
        fieldType.SetProperties(properties, actorId);
      }
    }
    if (payload.RelatedContent != null)
    {
      RelatedContentProperties properties = payload.RelatedContent.ToRelatedContentProperties(fieldType.RealmId);
      if (!reference.Properties.Equals(properties))
      {
        fieldType.SetProperties(properties, actorId);
      }
    }
    if (payload.RichText != null)
    {
      RichTextProperties properties = new(payload.RichText);
      if (!reference.Properties.Equals(properties))
      {
        fieldType.SetProperties(properties, actorId);
      }
    }
    if (payload.Select != null)
    {
      SelectProperties properties = payload.Select.ToSelectProperties();
      if (!reference.Properties.Equals(properties))
      {
        fieldType.SetProperties(properties, actorId);
      }
    }
    if (payload.String != null)
    {
      StringProperties properties = new(payload.String);
      if (!reference.Properties.Equals(properties))
      {
        fieldType.SetProperties(properties, actorId);
      }
    }
    if (payload.Tags != null)
    {
      TagsProperties properties = new(payload.Tags);
      if (!reference.Properties.Equals(properties))
      {
        fieldType.SetProperties(properties, actorId);
      }
    }
  }
}
