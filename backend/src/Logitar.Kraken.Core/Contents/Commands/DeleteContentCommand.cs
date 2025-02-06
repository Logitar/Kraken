using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Core.Localization;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Commands;

public record DeleteContentCommand(Guid Id, string? Language) : IRequest<ContentModel?>;

internal class DeleteContentCommandHandler : IRequestHandler<DeleteContentCommand, ContentModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;
  private readonly ILanguageManager _languageManager;

  public DeleteContentCommandHandler(
    IApplicationContext applicationContext,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    ILanguageManager languageManager)
  {
    _applicationContext = applicationContext;
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
    _languageManager = languageManager;
  }

  public async Task<ContentModel?> Handle(DeleteContentCommand command, CancellationToken cancellationToken)
  {
    ContentId contentId = new(_applicationContext.RealmId, command.Id);
    Content? content = await _contentRepository.LoadAsync(contentId, cancellationToken);
    if (content == null)
    {
      return null;
    }
    ContentModel model = await _contentQuerier.ReadAsync(content, cancellationToken);

    ActorId? actorId = _applicationContext.ActorId;
    if (string.IsNullOrWhiteSpace(command.Language))
    {
      content.Delete(actorId);
    }
    else
    {
      Language language = await _languageManager.FindAsync(command.Language, cancellationToken);
      if (!content.RemoveLocale(language, actorId))
      {
        return null;
      }

      int index = model.Locales.FindIndex(locale => locale.Language?.Id == language.EntityId);
      if (index >= 0)
      {
        model.Locales.RemoveAt(index);
      }
    }

    await _contentRepository.SaveAsync(content, cancellationToken);

    return model;
  }
}
