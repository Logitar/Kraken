using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Localization;
using MediatR;

namespace Logitar.Kraken.Core.Localization.Commands;

public record SetDefaultLanguageCommand(Guid Id) : IRequest<LanguageModel?>;

internal class SetDefaultLanguageCommandHandler : IRequestHandler<SetDefaultLanguageCommand, LanguageModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;

  public SetDefaultLanguageCommandHandler(IApplicationContext applicationContext, ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    _applicationContext = applicationContext;
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
  }

  public async Task<LanguageModel?> Handle(SetDefaultLanguageCommand command, CancellationToken cancellationToken)
  {
    LanguageId languageId = new(_applicationContext.RealmId, command.Id);
    Language? language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    if (language == null)
    {
      return null;
    }

    if (!language.IsDefault)
    {
      LanguageId defaultId = await _languageQuerier.FindDefaultIdAsync(cancellationToken);
      Language @default = await _languageRepository.LoadAsync(defaultId, cancellationToken)
        ?? throw new InvalidOperationException($"The default language 'Id={defaultId}' could not be loaded.");

      ActorId? actorId = _applicationContext.ActorId;
      @default.SetDefault(false, actorId);
      language.SetDefault(true, actorId);

      await _languageRepository.SaveAsync([@default, language], cancellationToken);
    }

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
