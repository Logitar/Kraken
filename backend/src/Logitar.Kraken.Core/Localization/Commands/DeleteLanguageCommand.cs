using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Dictionaries;
using MediatR;

namespace Logitar.Kraken.Core.Localization.Commands;

public record DeleteLanguageCommand(Guid Id) : IRequest<LanguageModel?>;

internal class DeleteLanguageCommandHandler : IRequestHandler<DeleteLanguageCommand, LanguageModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentRepository _contentRepository;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;

  public DeleteLanguageCommandHandler(
    IApplicationContext applicationContext,
    IContentRepository contentRepository,
    IDictionaryRepository dictionaryRepository,
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository)
  {
    _applicationContext = applicationContext;
    _contentRepository = contentRepository;
    _dictionaryRepository = dictionaryRepository;
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
  }

  public async Task<LanguageModel?> Handle(DeleteLanguageCommand command, CancellationToken cancellationToken)
  {
    LanguageId languageId = new(_applicationContext.RealmId, command.Id);
    Language? language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    if (language == null)
    {
      return null;
    }
    LanguageModel model = await _languageQuerier.ReadAsync(language, cancellationToken);

    ActorId? actorId = _applicationContext.ActorId;

    Dictionary? dictionary = await _dictionaryRepository.LoadAsync(language.Id, cancellationToken);
    if (dictionary != null)
    {
      dictionary.Delete(actorId);
      await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);
    }

    IReadOnlyCollection<Content> contents = await _contentRepository.LoadAsync(language.Id, cancellationToken);
    foreach (Content content in contents)
    {
      content.RemoveLocale(language, actorId);
    }
    await _contentRepository.SaveAsync(contents, cancellationToken);

    language.Delete(actorId);
    await _languageRepository.SaveAsync(language, cancellationToken);

    return model;
  }
}
