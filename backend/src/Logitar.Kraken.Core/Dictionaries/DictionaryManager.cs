using Logitar.Kraken.Core.Dictionaries.Events;

namespace Logitar.Kraken.Core.Dictionaries;

internal class DictionaryManager : IDictionaryManager
{
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly IDictionaryRepository _dictionaryRepository;

  public DictionaryManager(IDictionaryQuerier dictionaryQuerier, IDictionaryRepository dictionaryRepository)
  {
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken)
  {
    bool hasLocaleChanged = dictionary.Changes.Any(change => change is DictionaryCreated || change is DictionaryLanguageChanged);
    if (hasLocaleChanged)
    {
      DictionaryId? conflictId = await _dictionaryQuerier.FindIdAsync(dictionary.LanguageId, cancellationToken);
      if (conflictId.HasValue && !conflictId.Equals(dictionary.Id))
      {
        throw new LanguageAlreadyUsedException(dictionary, conflictId.Value);
      }
    }

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);
  }
}
