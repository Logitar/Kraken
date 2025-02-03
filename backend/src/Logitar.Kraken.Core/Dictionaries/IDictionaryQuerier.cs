using Logitar.Kraken.Contracts.Dictionaries;
using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Core.Dictionaries;

public interface IDictionaryQuerier
{
  Task<DictionaryModel> ReadAsync(Dictionary dictionary, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> ReadAsync(DictionaryId id, CancellationToken cancellationToken = default);
  Task<DictionaryModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<DictionaryModel>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken = default);
}
