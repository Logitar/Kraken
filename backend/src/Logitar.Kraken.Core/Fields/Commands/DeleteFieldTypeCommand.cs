using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using MediatR;

namespace Logitar.Kraken.Core.Fields.Commands;

public record DeleteFieldTypeCommand(Guid Id) : Activity, IRequest<FieldTypeModel?>;

internal class DeleteFieldTypeCommandHandler : IRequestHandler<DeleteFieldTypeCommand, FieldTypeModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public DeleteFieldTypeCommandHandler(
    IApplicationContext applicationContext,
    IContentTypeRepository contentTypeRepository,
    IFieldTypeQuerier fieldTypeQuerier,
    IFieldTypeRepository fieldTypeRepository)
  {
    _applicationContext = applicationContext;
    _contentTypeRepository = contentTypeRepository;
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task<FieldTypeModel?> Handle(DeleteFieldTypeCommand command, CancellationToken cancellationToken)
  {
    FieldTypeId fieldTypeId = new(_applicationContext.RealmId, command.Id);
    FieldType? fieldType = await _fieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken);
    if (fieldType == null)
    {
      return null;
    }
    FieldTypeModel result = await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);

    ActorId? actorId = _applicationContext.ActorId;

    IReadOnlyCollection<ContentType> contentTypes = await _contentTypeRepository.LoadAsync(fieldType.Id, cancellationToken);
    foreach (ContentType contentType in contentTypes)
    {
      FieldDefinition[] fieldDefinitions = [.. contentType.FieldDefinitions];
      foreach (FieldDefinition fieldDefinition in fieldDefinitions)
      {
        if (fieldDefinition.FieldTypeId == fieldType.Id)
        {
          contentType.RemoveField(fieldDefinition, actorId);
        }
      }
    }
    await _contentTypeRepository.SaveAsync(contentTypes, cancellationToken);

    fieldType.Delete(actorId);
    await _fieldTypeRepository.SaveAsync(fieldType, cancellationToken);

    return result;
  }
}
