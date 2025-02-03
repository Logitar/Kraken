using FluentValidation;
using FluentValidation.Results;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields.Validators;
using Logitar.Kraken.Core.Realms;
using MediatR;

namespace Logitar.Kraken.Core.Fields.Commands;

/// <exception cref="FieldTypeNotFoundException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceFieldDefinitionCommand(Guid ContentTypeId, Guid? FieldId, CreateOrReplaceFieldDefinitionPayload Payload) : IRequest<ContentTypeModel?>;

internal class CreateOrReplaceFieldDefinitionCommandHandler : IRequestHandler<CreateOrReplaceFieldDefinitionCommand, ContentTypeModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public CreateOrReplaceFieldDefinitionCommandHandler(
    IApplicationContext applicationContext,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository,
    IFieldTypeRepository fieldTypeRepository)
  {
    _applicationContext = applicationContext;
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task<ContentTypeModel?> Handle(CreateOrReplaceFieldDefinitionCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldDefinitionPayload payload = command.Payload;
    new CreateOrReplaceFieldDefinitionValidator().ValidateAndThrow(payload);

    RealmId? realmId = _applicationContext.RealmId;
    ContentTypeId contentTypeId = new(realmId, command.ContentTypeId);
    ContentType? contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType == null)
    {
      return null;
    }

    FieldTypeId? fieldTypeId = command.FieldId.HasValue ? contentType.TryGetField(command.FieldId.Value)?.FieldTypeId : null;
    if (fieldTypeId == null)
    {
      if (!payload.FieldTypeId.HasValue)
      {
        ValidationFailure failure = new(nameof(payload.FieldTypeId), "'FieldTypeId' is required when creating a field definition.", payload.FieldTypeId)
        {
          ErrorCode = "RequiredValidator"
        };
        throw new ValidationException([failure]);
      }

      fieldTypeId = new(realmId, payload.FieldTypeId.Value);
      _ = await _fieldTypeRepository.LoadAsync(fieldTypeId.Value, cancellationToken) ?? throw new FieldTypeNotFoundException(fieldTypeId.Value, nameof(payload.FieldTypeId));
    }

    Guid fieldId = command.FieldId ?? Guid.NewGuid();
    Identifier uniqueName = new(payload.UniqueName);
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    Description? description = Description.TryCreate(payload.Description);
    Placeholder? placeholder = Placeholder.TryCreate(payload.Placeholder);
    FieldDefinition fieldDefinition = new(fieldId, fieldTypeId.Value, payload.IsInvariant, payload.IsRequired, payload.IsIndexed, payload.IsUnique, uniqueName, displayName, description, placeholder);
    contentType.SetField(fieldDefinition, _applicationContext.ActorId);

    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}
