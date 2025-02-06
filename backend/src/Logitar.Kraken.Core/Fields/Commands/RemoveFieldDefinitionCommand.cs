using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Core.Contents;
using MediatR;

namespace Logitar.Kraken.Core.Fields.Commands;

public record RemoveFieldDefinitionCommand(Guid ContentTypeId, Guid FieldId) : IRequest<ContentTypeModel?>;

internal class RemoveFieldDefinitionCommandHandler : IRequestHandler<RemoveFieldDefinitionCommand, ContentTypeModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;

  public RemoveFieldDefinitionCommandHandler(IApplicationContext applicationContext, IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository)
  {
    _applicationContext = applicationContext;
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
  }

  public async Task<ContentTypeModel?> Handle(RemoveFieldDefinitionCommand command, CancellationToken cancellationToken)
  {
    ContentTypeId contentTypeId = new(_applicationContext.RealmId, command.ContentTypeId);
    ContentType? contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType == null || !contentType.RemoveField(command.FieldId, _applicationContext.ActorId))
    {
      return null;
    }

    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}
