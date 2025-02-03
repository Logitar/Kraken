using FluentValidation;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents.Events;
using Logitar.Kraken.Core.Fields;
using Logitar.Kraken.Core.Fields.Validators;
using Logitar.Kraken.Core.Localization;

namespace Logitar.Kraken.Core.Contents;

internal class ContentManager : IContentManager
{
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly IFieldValueValidatorFactory _fieldValueValidatorFactory;

  public ContentManager(
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    IContentTypeRepository contentTypeRepository,
    IFieldTypeRepository fieldTypeRepository,
    IFieldValueValidatorFactory fieldValueValidatorFactory)
  {
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
    _contentTypeRepository = contentTypeRepository;
    _fieldTypeRepository = fieldTypeRepository;
    _fieldValueValidatorFactory = fieldValueValidatorFactory;
  }

  public async Task SaveAsync(Content content, CancellationToken cancellationToken)
  {
    ContentType contentType = await _contentTypeRepository.LoadAsync(content, cancellationToken);
    await SaveAsync(content, contentType, cancellationToken);
  }
  public async Task SaveAsync(Content content, ContentType contentType, CancellationToken cancellationToken)
  {
    if (contentType.Id != content.ContentTypeId)
    {
      throw new ArgumentException($"The content type 'Id={contentType.Id}' was not expected. The expected content type for content 'Id={content.Id}' is '{content.ContentTypeId}'.", nameof(contentType));
    }

    HashSet<LanguageId?> languageIds = new(capacity: content.Locales.Count + 1);
    foreach (IEvent change in content.Changes)
    {
      if (change is ContentCreated)
      {
        languageIds.Add(null);
      }
      else if (change is ContentLocaleChanged localeChanged)
      {
        languageIds.Add(localeChanged.LanguageId);
      }
      else if (change is ContentLocalePublished localePublished)
      {
        languageIds.Add(localePublished.LanguageId);
      }
    }

    if (languageIds.Count > 0)
    {
      HashSet<FieldTypeId> fieldTypeIds = contentType.FieldDefinitions.Select(x => x.FieldTypeId).ToHashSet();
      Dictionary<FieldTypeId, FieldType> fieldTypes = (await _fieldTypeRepository.LoadAsync(fieldTypeIds, cancellationToken))
        .ToDictionary(x => x.Id, x => x);

      foreach (LanguageId? languageId in languageIds)
      {
        ContentLocale invariantOrLocale = languageId.HasValue ? content.FindLocale(languageId.Value) : content.Invariant;

        ContentId? conflictId = await _contentQuerier.FindIdAsync(content.ContentTypeId, languageId, invariantOrLocale.UniqueName, cancellationToken);
        if (conflictId.HasValue && !conflictId.Value.Equals(content.Id))
        {
          throw new ContentUniqueNameAlreadyUsedException(content, languageId, invariantOrLocale, conflictId.Value);
        }

        bool isPublished = content.IsPublished(languageId);
        await ValidateAsync(contentType, fieldTypes, content.Id, languageId, invariantOrLocale.FieldValues, isPublished, cancellationToken);
      }
    }

    await _contentRepository.SaveAsync(content, cancellationToken);
  }

  private async Task ValidateAsync(
    ContentType contentType,
    Dictionary<FieldTypeId, FieldType> fieldTypes,
    ContentId contentId,
    LanguageId? languageId,
    IReadOnlyDictionary<Guid, string> fieldValues,
    bool isPublished,
    CancellationToken cancellationToken)
  {
    const string PropertyName = "FieldValues";

    bool isInvariant = languageId == null;
    int capacity = contentType.FieldDefinitions.Count;
    Dictionary<Guid, FieldDefinition> fieldDefinitions = new(capacity);
    HashSet<Guid> requiredIds = new(capacity);
    HashSet<Guid> uniqueIds = new(capacity);
    foreach (FieldDefinition fieldDefinition in contentType.FieldDefinitions)
    {
      if (fieldDefinition.IsInvariant == isInvariant)
      {
        fieldDefinitions[fieldDefinition.Id] = fieldDefinition;

        if (fieldDefinition.IsRequired)
        {
          requiredIds.Add(fieldDefinition.Id);
        }
        if (fieldDefinition.IsUnique)
        {
          uniqueIds.Add(fieldDefinition.Id);
        }
      }
    }

    capacity = fieldValues.Count;
    List<Guid> unexpectedIds = new(capacity);
    List<ValidationFailure> validationFailures = [];
    Dictionary<Guid, string> uniqueValues = new(capacity);
    foreach (KeyValuePair<Guid, string> fieldValue in fieldValues)
    {
      if (!fieldDefinitions.TryGetValue(fieldValue.Key, out FieldDefinition? fieldDefinition))
      {
        unexpectedIds.Add(fieldValue.Key);
        continue;
      }

      requiredIds.Remove(fieldValue.Key);

      FieldType fieldType = fieldTypes[fieldDefinition.FieldTypeId];
      IFieldValueValidator validator = _fieldValueValidatorFactory.Create(fieldType);
      ValidationResult result = await validator.ValidateAsync(fieldValue.Value, $"{PropertyName}.{fieldValue.Key}", cancellationToken);
      if (!result.IsValid)
      {
        validationFailures.AddRange(result.Errors);
        continue;
      }

      if (uniqueIds.Contains(fieldValue.Key))
      {
        uniqueValues[fieldValue.Key] = fieldValue.Value;
      }
    }

    if (unexpectedIds.Count > 0)
    {
      IEnumerable<ValidationFailure> unexpectedFailures = unexpectedIds.Select(id => new ValidationFailure(PropertyName, "The specified field was not expected.", id)
      {
        ErrorCode = "UnexpectedFieldValidator"
      });
      throw new ValidationException(unexpectedFailures);
    }
    if (isPublished && requiredIds.Count > 0)
    {
      IEnumerable<ValidationFailure> requiredFailures = requiredIds.Select(id => new ValidationFailure(PropertyName, "The specified field is missing.", id)
      {
        ErrorCode = "RequiredFieldValidator"
      });
      throw new ValidationException(requiredFailures);
    }
    if (validationFailures.Count > 0)
    {
      throw new ValidationException(validationFailures);
    }

    if (uniqueValues.Count > 0)
    {
      IReadOnlyDictionary<Guid, ContentId> conflicts = await _contentQuerier.FindConflictsAsync(contentType.Id, languageId, uniqueValues, contentId, cancellationToken);
      if (conflicts.Count > 0)
      {
        throw new ContentFieldValueConflictException(contentId, conflicts, PropertyName);
      }
    }
  }
}
