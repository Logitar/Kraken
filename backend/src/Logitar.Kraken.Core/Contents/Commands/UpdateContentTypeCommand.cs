using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Core.Contents.Validators;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Commands;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdateContentTypeCommand(Guid Id, UpdateContentTypePayload Payload) : IRequest<ContentTypeModel?>;

internal class UpdateContentTypeCommandHandler : IRequestHandler<UpdateContentTypeCommand, ContentTypeModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentTypeManager _contentTypeManager;
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;

  public UpdateContentTypeCommandHandler(
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

  public async Task<ContentTypeModel?> Handle(UpdateContentTypeCommand command, CancellationToken cancellationToken)
  {
    UpdateContentTypePayload payload = command.Payload;
    new UpdateContentTypeValidator().ValidateAndThrow(payload);

    ContentTypeId contentTypeId = new(_applicationContext.RealmId, command.Id);
    ContentType? contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    if (payload.IsInvariant.HasValue)
    {
      contentType.IsInvariant = payload.IsInvariant.Value;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      Identifier uniqueName = new(payload.UniqueName);
      contentType.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName != null)
    {
      contentType.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      contentType.Description = Description.TryCreate(payload.Description.Value);
    }

    contentType.Update(actorId);

    await _contentTypeManager.SaveAsync(contentType, cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}
