using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Core.Contents.Validators;
using Logitar.Kraken.Core.Realms;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Commands;

public record CreateOrReplaceContentTypeResult(ContentTypeModel? ContentType = null, bool Created = false);

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceContentTypeCommand(Guid? Id, CreateOrReplaceContentTypePayload Payload, long? Version) : IRequest<CreateOrReplaceContentTypeResult>;

internal class CreateOrReplaceContentTypeCommandHandler : IRequestHandler<CreateOrReplaceContentTypeCommand, CreateOrReplaceContentTypeResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentTypeManager _contentTypeManager;
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;

  public CreateOrReplaceContentTypeCommandHandler(
    IApplicationContext applicationContext,
    IContentTypeManager contentTypeManager,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository)
  {
    _applicationContext = applicationContext;
    _contentTypeManager = contentTypeManager;
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
  }

  public async Task<CreateOrReplaceContentTypeResult> Handle(CreateOrReplaceContentTypeCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentTypePayload payload = command.Payload;
    new CreateOrReplaceContentTypeValidator().ValidateAndThrow(payload);

    Identifier uniqueName = new(payload.UniqueName);
    ActorId? actorId = _applicationContext.ActorId;

    RealmId? realmId = _applicationContext.RealmId;
    ContentTypeId contentTypeId = ContentTypeId.NewId(realmId);
    ContentType? contentType = null;
    if (command.Id.HasValue)
    {
      contentTypeId = new(realmId, command.Id.Value);
      contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    }

    bool created = false;
    if (contentType == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceContentTypeResult();
      }

      contentType = new(uniqueName, payload.IsInvariant, actorId, contentTypeId);
      created = true;
    }

    ContentType reference = (command.Version.HasValue
      ? await _contentTypeRepository.LoadAsync(contentType.Id, command.Version.Value, cancellationToken)
      : null) ?? contentType;

    if (!created)
    {
      if (reference.IsInvariant != payload.IsInvariant)
      {
        contentType.IsInvariant = payload.IsInvariant;
      }

      if (reference.UniqueName != uniqueName)
      {
        contentType.SetUniqueName(uniqueName, actorId);
      }
    }

    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      contentType.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      contentType.Description = description;
    }

    contentType.Update(actorId);

    await _contentTypeManager.SaveAsync(contentType, cancellationToken);

    ContentTypeModel model = await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
    return new CreateOrReplaceContentTypeResult(model, created);
  }
}
