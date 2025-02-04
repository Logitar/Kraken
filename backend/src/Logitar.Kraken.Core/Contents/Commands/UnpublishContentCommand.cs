using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Realms;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Commands;

public record UnpublishContentCommand : IRequest<ContentModel?>
{
  public Guid ContentId { get; }
  public Guid? LanguageId { get; }
  public bool All { get; }

  public UnpublishContentCommand(Guid contentId)
  {
    ContentId = contentId;
    All = true;
  }

  public UnpublishContentCommand(Guid contentId, Guid? languageId)
  {
    ContentId = contentId;
    LanguageId = languageId;
  }
}

// TODO(fpion): Language (Id or Locale code)

internal class UnpublishContentCommandHandler : IRequestHandler<UnpublishContentCommand, ContentModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentManager _contentManager;
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;

  public UnpublishContentCommandHandler(
    IApplicationContext applicationContext,
    IContentManager contentManager,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository)
  {
    _applicationContext = applicationContext;
    _contentManager = contentManager;
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
  }

  public async Task<ContentModel?> Handle(UnpublishContentCommand command, CancellationToken cancellationToken)
  {
    RealmId? realmId = _applicationContext.RealmId;
    ContentId contentId = new(realmId, command.ContentId);
    Content? content = await _contentRepository.LoadAsync(contentId, cancellationToken);
    if (content == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;
    if (command.All)
    {
      content.Unpublish(actorId);
    }
    else if (command.LanguageId.HasValue)
    {
      LanguageId languageId = new(realmId, command.LanguageId.Value);
      if (!content.UnpublishLocale(languageId, actorId))
      {
        return null;
      }
    }
    else
    {
      content.UnpublishInvariant(actorId);
    }

    await _contentManager.SaveAsync(content, cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}
