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

  public async Task<Language> FindAsync(string idOrLocaleCode, CancellationToken cancellationToken)
  {
    RealmId? realmId = _applicationContext.RealmId;
    Language? language = null;
    if (Guid.TryParse(idOrLocaleCode.Trim(), out Guid entityId))
    {
      LanguageId languageId = new(entityId, realmId);
      language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    }
    else
    {
      Locale? locale = null;
      try
      {
        locale = new(idOrLocaleCode);
      }
      catch (Exception)
      {
      }

      if (locale != null)
      {
        LanguageId? languageId = await _languageQuerier.FindIdAsync(locale, cancellationToken);
        if (languageId.HasValue)
        {
          language = await _languageRepository.LoadAsync(languageId.Value, cancellationToken);
        }
      }
    }

    if (language == null)
    {
      throw new LanguageNotFoundException(realmId, idOrLocaleCode, PropertyName);
    }

    return language;
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
