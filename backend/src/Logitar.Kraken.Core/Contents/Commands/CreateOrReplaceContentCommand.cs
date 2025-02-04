using FluentValidation;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents.Validators;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Commands;

public record CreateOrReplaceContentResult(ContentModel? Content = null, bool Created = false);

/// <exception cref="ContentFieldValueConflictException"></exception>
/// <exception cref="ContentTypeNotFoundException"></exception>
/// <exception cref="ContentUniqueNameAlreadyUsedException"></exception>
/// <exception cref="LanguageNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceContentCommand(Guid? ContentId, Guid? LanguageId, CreateOrReplaceContentPayload Payload) : IRequest<CreateOrReplaceContentResult>;

// TODO(fpion): Language (Id or Locale code)

internal class CreateOrReplaceContentCommandHandler : IRequestHandler<CreateOrReplaceContentCommand, CreateOrReplaceContentResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentManager _contentManager;
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;

  public CreateOrReplaceContentCommandHandler(
    IApplicationContext applicationContext,
    IContentManager contentManager,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    IContentTypeRepository contentTypeRepository,
    ILanguageRepository languageRepository)
  {
    _applicationContext = applicationContext;
    _contentManager = contentManager;
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
    _contentTypeRepository = contentTypeRepository;
    _languageRepository = languageRepository;
  }

  public async Task<CreateOrReplaceContentResult> Handle(CreateOrReplaceContentCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentPayload payload = command.Payload;
    new CreateOrReplaceContentValidator().ValidateAndThrow(payload);

    RealmId? realmId = _applicationContext.RealmId;
    ContentId contentId = ContentId.NewId(realmId);
    Content? content = null;
    if (command.ContentId.HasValue)
    {
      contentId = new(realmId, command.ContentId.Value);
      content = await _contentRepository.LoadAsync(contentId, cancellationToken);
    }

    ContentType contentType;
    bool created = false;
    if (content == null)
    {
      if (!payload.ContentTypeId.HasValue)
      {
        ValidationFailure failure = new(nameof(payload.ContentTypeId), "'ContentTypeId' is required when creating content.", payload.ContentTypeId)
        {
          ErrorCode = "RequiredValidator"
        };
        throw new ValidationException([failure]);
      }

      ContentTypeId contentTypeId = new(realmId, payload.ContentTypeId.Value);
      contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken)
        ?? throw new ContentTypeNotFoundException(contentTypeId, nameof(payload.ContentTypeId));

      content = await CreateAsync(payload, contentType, command.LanguageId, contentId, cancellationToken);
      created = true;
    }
    else
    {
      contentType = await _contentTypeRepository.LoadAsync(content, cancellationToken);

      await ReplaceAsync(content, payload, contentType, command.LanguageId, cancellationToken);
    }

    await _contentManager.SaveAsync(content, contentType, cancellationToken);

    ContentModel model = await _contentQuerier.ReadAsync(content, cancellationToken);
    return new CreateOrReplaceContentResult(model, created);
  }

  private async Task<Content> CreateAsync(CreateOrReplaceContentPayload payload, ContentType contentType, Guid? languageGuid, ContentId? contentId, CancellationToken cancellationToken)
  {
    string? errorMessage = null;
    if (contentType.IsInvariant && languageGuid.HasValue)
    {
      errorMessage = "'LanguageId' must be null. The content type is invariant.";
    }
    else if (!contentType.IsInvariant && !languageGuid.HasValue)
    {
      errorMessage = "'LanguageId' cannot be null. The content type is not invariant.";
    }
    if (errorMessage != null)
    {
      ValidationFailure failure = new("LanguageId", errorMessage, languageGuid)
      {
        ErrorCode = "InvariantValidator"
      };
      throw new ValidationException([failure]);
    }

    ActorId? actorId = _applicationContext.ActorId;

    ContentLocale invariant = CreateLocale(payload, contentType, language: null);
    Content content = new(contentType, invariant, actorId, contentId);

    if (languageGuid.HasValue)
    {
      LanguageId languageId = new(_applicationContext.RealmId, languageGuid.Value);
      Language language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new LanguageNotFoundException(languageId, "LanguageId");

      ContentLocale locale = CreateLocale(payload, contentType, language);
      content.SetLocale(language, locale, actorId);
    }

    return content;
  }

  private async Task ReplaceAsync(Content content, CreateOrReplaceContentPayload payload, ContentType contentType, Guid? languageGuid, CancellationToken cancellationToken)
  {
    ContentLocale invariantOrLocale = CreateLocale(payload);
    ActorId? actorId = _applicationContext.ActorId;

    if (languageGuid.HasValue)
    {
      if (contentType.IsInvariant)
      {
        ValidationFailure failure = new("LanguageId", "'LanguageId' must be null. The content type is invariant.", languageGuid)
        {
          ErrorCode = "InvariantValidator"
        };
        throw new ValidationException([failure]);
      }

      LanguageId languageId = new(_applicationContext.RealmId, languageGuid.Value);
      Language language = await _languageRepository.LoadAsync(languageId, cancellationToken)
        ?? throw new LanguageNotFoundException(languageId, "LanguageId");

      content.SetLocale(language, invariantOrLocale, actorId);
    }
    else
    {
      content.SetInvariant(invariantOrLocale, actorId);
    }
  }

  private static ContentLocale CreateLocale(CreateOrReplaceContentPayload payload)
  {
    UniqueName uniqueName = new(Content.UniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    Description? description = Description.TryCreate(payload.Description);

    Dictionary<Guid, string> fieldValues = new(capacity: payload.FieldValues.Count);
    foreach (FieldValue fieldValue in payload.FieldValues)
    {
      fieldValues[fieldValue.Id] = fieldValue.Value;
    }

    return new ContentLocale(uniqueName, displayName, description, fieldValues);
  }
  private static ContentLocale CreateLocale(CreateOrReplaceContentPayload payload, ContentType contentType, Language? language)
  {
    UniqueName uniqueName = new(Content.UniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    Description? description = Description.TryCreate(payload.Description);

    HashSet<Guid> variantFieldIds = contentType.FieldDefinitions.Where(x => !x.IsInvariant).Select(x => x.Id).ToHashSet();
    Dictionary<Guid, string> fieldValues = new(capacity: payload.FieldValues.Count);
    if (language == null)
    {
      foreach (FieldValue fieldValue in payload.FieldValues)
      {
        if (!variantFieldIds.Contains(fieldValue.Id))
        {
          fieldValues[fieldValue.Id] = fieldValue.Value;
        }
      }
    }
    else
    {
      foreach (FieldValue fieldValue in payload.FieldValues)
      {
        if (variantFieldIds.Contains(fieldValue.Id))
        {
          fieldValues[fieldValue.Id] = fieldValue.Value;
        }
      }
    }

    return new ContentLocale(uniqueName, displayName, description, fieldValues);
  }
}
