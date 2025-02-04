using Logitar.Kraken.Core.Dictionaries.Events;

namespace Logitar.Kraken.Core.Dictionaries;

internal class DictionaryManager : IDictionaryManager
{
  private readonly IDictionaryRepository _dictionaryRepository;

  public DictionaryManager(IDictionaryRepository dictionaryRepository)
  {
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken)
  {
    bool hasLocaleChanged = dictionary.Changes.Any(change => change is DictionaryCreated || change is DictionaryLanguageChanged);
    if (hasLocaleChanged)
    {
      //Dictionary? conflict = await _dictionaryRepository.LoadAsync(dictionary.RealmId, dictionary.Locale, cancellationToken);
      //if (conflict != null && !conflict.Equals(dictionary))
      //{
      //  throw new DictionaryAlreadyExistsException(dictionary, conflict.Id);
      //} // ISSUE #42: https://github.com/Logitar/Kraken/issues/42
    }

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);
  }
}
