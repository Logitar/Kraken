using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Commands;

public record DeleteContentTypeCommand(Guid Id) : IRequest<ContentTypeModel?>;

internal class DeleteContentTypeCommandHandler : IRequestHandler<DeleteContentTypeCommand, ContentTypeModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;

  public DeleteContentTypeCommandHandler(
    IApplicationContext applicationContext,
    IContentRepository contentRepository,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository)
  {
    _applicationContext = applicationContext;
    _contentRepository = contentRepository;
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
  }

  public async Task<ContentTypeModel?> Handle(DeleteContentTypeCommand command, CancellationToken cancellationToken)
  {
    ContentTypeId contentTypeId = new(_applicationContext.RealmId, command.Id);
    ContentType? contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    IReadOnlyCollection<Content> contents = await _contentRepository.LoadAsync(contentType.Id, cancellationToken);
    foreach (Content content in contents)
    {
      content.Delete(actorId);
    }
    await _contentRepository.SaveAsync(contents, cancellationToken);

    contentType.Delete(actorId);
    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}
