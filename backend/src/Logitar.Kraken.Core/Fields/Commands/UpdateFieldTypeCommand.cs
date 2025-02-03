using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields.Properties;
using Logitar.Kraken.Core.Fields.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Fields.Commands;

/// <exception cref="ContentTypeNotFoundException"></exception>
/// <exception cref="UnexpectedFieldTypePropertiesException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdateFieldTypeCommand(Guid Id, UpdateFieldTypePayload Payload) : IRequest<FieldTypeModel?>;

internal class UpdateFieldTypeCommandHandler : IRequestHandler<UpdateFieldTypeCommand, FieldTypeModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IFieldTypeManager _fieldTypeManager;
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public UpdateFieldTypeCommandHandler(
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

  public async Task<FieldTypeModel?> Handle(UpdateFieldTypeCommand command, CancellationToken cancellationToken)
  {
    UpdateFieldTypePayload payload = command.Payload;
    new UpdateFieldTypeValidator().ValidateAndThrow(payload);

    FieldTypeId fieldTypeId = new(_applicationContext.RealmId, command.Id);
    FieldType? fieldType = await _fieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken);
    if (fieldType == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(FieldType.UniqueNameSettings, payload.UniqueName);
      fieldType.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName != null)
    {
      fieldType.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      fieldType.Description = Description.TryCreate(payload.Description.Value);
    }

    fieldType.Update(actorId);

    SetProperties(payload, fieldType, actorId);

    await _fieldTypeManager.SaveAsync(fieldType, cancellationToken);

    return await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
  }

  private static void SetProperties(UpdateFieldTypePayload payload, FieldType fieldType, ActorId? actorId)
  {
    if (payload.Boolean != null)
    {
      BooleanProperties settings = new(payload.Boolean);
      fieldType.SetProperties(settings, actorId);
    }
    if (payload.DateTime != null)
    {
      DateTimeProperties settings = new(payload.DateTime);
      fieldType.SetProperties(settings, actorId);
    }
    if (payload.Number != null)
    {
      NumberProperties settings = new(payload.Number);
      fieldType.SetProperties(settings, actorId);
    }
    if (payload.RelatedContent != null)
    {
      RelatedContentProperties settings = payload.RelatedContent.ToRelatedContentProperties(fieldType.RealmId);
      fieldType.SetProperties(settings, actorId);
    }
    if (payload.RichText != null)
    {
      RichTextProperties settings = new(payload.RichText);
      fieldType.SetProperties(settings, actorId);
    }
    if (payload.Select != null)
    {
      SelectProperties settings = payload.Select.ToSelectProperties();
      fieldType.SetProperties(settings, actorId);
    }
    if (payload.String != null)
    {
      StringProperties settings = new(payload.String);
      fieldType.SetProperties(settings, actorId);
    }
    if (payload.Tags != null)
    {
      TagsProperties settings = new(payload.Tags);
      fieldType.SetProperties(settings, actorId);
    }
  }
}
