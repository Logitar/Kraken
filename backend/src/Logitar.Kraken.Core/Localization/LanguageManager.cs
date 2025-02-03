using Logitar.Kraken.Core.Localization.Events;

namespace Logitar.Kraken.Core.Localization;

internal class LanguageManager : ILanguageManager
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;

  public LanguageManager(ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
  }

  public async Task SaveAsync(Language language, CancellationToken cancellationToken)
  {
    bool hasLocaleChanged = language.Changes.Any(change => change is LanguageCreated || change is LanguageLocaleChanged);
    if (hasLocaleChanged)
    {
      LanguageId? conflictId = await _languageQuerier.FindIdAsync(language.Locale, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(language.Id))
      {
        throw new LocaleAlreadyUsedException(language, conflictId.Value);
      }
    }

    await _languageRepository.SaveAsync(language, cancellationToken);
  }
}
