using Logitar.Kraken.Contracts.Dictionaries;
using Logitar.Kraken.Contracts.Search;
using MediatR;

namespace Logitar.Kraken.Core.Dictionaries.Queries;

public record SearchDictionariesQuery(SearchDictionariesPayload Payload) : Activity, IRequest<SearchResults<DictionaryModel>>;

internal class SearchDictionariesQueryHandler : IRequestHandler<SearchDictionariesQuery, SearchResults<DictionaryModel>>
{
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public SearchDictionariesQueryHandler(IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<SearchResults<DictionaryModel>> Handle(SearchDictionariesQuery query, CancellationToken cancellationToken)
  {
    return await _dictionaryQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
