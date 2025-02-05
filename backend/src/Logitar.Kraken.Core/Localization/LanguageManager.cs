using Logitar.Kraken.Core.Localization.Events;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Localization;

internal class LanguageManager : ILanguageManager
{
  private const string PropertyName = "Language";

  private readonly IApplicationContext _applicationContext;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;

  public LanguageManager(IApplicationContext applicationContext, ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    _applicationContext = applicationContext;
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
  }

  public async Task<Language> FindAsync(string language, CancellationToken cancellationToken)
  {
    RealmId? realmId = _applicationContext.RealmId;
    Language? found = null;
    if (Guid.TryParse(language.Trim(), out Guid entityId))
    {
      LanguageId languageId = new(realmId, entityId);
      found = await _languageRepository.LoadAsync(languageId, cancellationToken);
    }
    else
    {
      Locale? locale = null;
      try
      {
        locale = new(language);
      }
      catch (Exception)
      {
      }

      if (locale != null)
      {
        found = await _languageRepository.LoadAsync(realmId, locale, cancellationToken);
      }
    }

    if (found == null)
    {
      throw new LanguageNotFoundException(realmId, language, PropertyName);
    }

    return found;
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
