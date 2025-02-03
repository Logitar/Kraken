using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Fields.Events;
using Logitar.Kraken.Core.Fields.Properties;

namespace Logitar.Kraken.Core.Fields;

internal class FieldTypeManager : IFieldTypeManager
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public FieldTypeManager(IContentTypeQuerier contentTypeQuerier, IFieldTypeQuerier fieldTypeQuerier, IFieldTypeRepository fieldTypeRepository)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = false;
    RelatedContentProperties? relatedContentProperties = null;
    foreach (IEvent change in fieldType.Changes)
    {
      if (change is FieldTypeCreated || change is FieldTypeUniqueNameChanged)
      {
        hasUniqueNameChanged = true;
      }
      else if (change is FieldTypeRelatedContentPropertiesChanged relatedContentPropertiesChanged)
      {
        relatedContentProperties = relatedContentPropertiesChanged.Properties;
      }
    }

    if (hasUniqueNameChanged)
    {
      FieldTypeId? conflictId = await _fieldTypeQuerier.FindIdAsync(fieldType.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(fieldType.Id))
      {
        throw new UniqueNameAlreadyUsedException(fieldType, conflictId.Value);
      }
    }

    if (relatedContentProperties != null)
    {
      _ = await _contentTypeQuerier.ReadAsync(relatedContentProperties.ContentTypeId, cancellationToken)
        ?? throw new ContentTypeNotFoundException(relatedContentProperties.ContentTypeId, nameof(relatedContentProperties.ContentTypeId));
    }

    await _fieldTypeRepository.SaveAsync(fieldType, cancellationToken);
  }
}
