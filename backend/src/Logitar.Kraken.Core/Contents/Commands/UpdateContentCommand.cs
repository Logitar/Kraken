using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents.Validators;
using Logitar.Kraken.Core.Localization;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Commands;

/// <exception cref="ContentFieldValueConflictException"></exception>
/// <exception cref="ContentUniqueNameAlreadyUsedException"></exception>
/// <exception cref="LanguageNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdateContentCommand(Guid ContentId, Guid? LanguageId, UpdateContentPayload Payload) : IRequest<ContentModel?>;

// TODO(fpion): language (Id or Locale code)

internal class UpdateContentCommandHandler : IRequestHandler<UpdateContentCommand, ContentModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentManager _contentManager;
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly ILanguageRepository _languageRepository;

  public UpdateContentCommandHandler(
    IApplicationContext applicationContext,
    IContentManager contentManager,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    ILanguageRepository languageRepository)
  {
    _applicationContext = applicationContext;
    _contentManager = contentManager;
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
    _languageRepository = languageRepository;
  }

  public async Task<ContentModel?> Handle(UpdateContentCommand command, CancellationToken cancellationToken)
  {
    UpdateContentPayload payload = command.Payload;
    new UpdateContentValidator().ValidateAndThrow(payload);

    ContentId contentId = new(_applicationContext.RealmId, command.ContentId);
    Content? content = await _contentRepository.LoadAsync(contentId, cancellationToken);
    if (content == null)
    {
      return null;
    }

    ContentLocale? invariantOrLocale;
    Language? language = null;
    if (command.LanguageId.HasValue)
    {
      LanguageId languageId = new(_applicationContext.RealmId, command.LanguageId.Value);
      language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new LanguageNotFoundException(languageId, nameof(command.LanguageId));

      invariantOrLocale = content.TryGetLocale(language);
      if (invariantOrLocale == null)
      {
        return null;
      }
    }
    else
    {
      invariantOrLocale = content.Invariant;
    }

    UniqueName uniqueName = string.IsNullOrWhiteSpace(payload.UniqueName) ? invariantOrLocale.UniqueName : new(Content.UniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = payload.DisplayName == null ? invariantOrLocale.DisplayName : DisplayName.TryCreate(payload.DisplayName.Value);
    Description? description = payload.Description == null ? invariantOrLocale.Description : Description.TryCreate(payload.Description.Value);

    Dictionary<Guid, string> fieldValues = new(invariantOrLocale.FieldValues);
    foreach (FieldValueUpdate fieldValue in payload.FieldValues)
    {
      if (string.IsNullOrWhiteSpace(fieldValue.Value))
      {
        fieldValues.Remove(fieldValue.Id);
      }
      else
      {
        fieldValues[fieldValue.Id] = fieldValue.Value;
      }
    }

    invariantOrLocale = new(uniqueName, displayName, description, fieldValues);
    ActorId? actorId = _applicationContext.ActorId;

    if (language == null)
    {
      content.SetInvariant(invariantOrLocale, actorId);
    }
    else
    {
      content.SetLocale(language, invariantOrLocale, actorId);
    }

    await _contentManager.SaveAsync(content, cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
