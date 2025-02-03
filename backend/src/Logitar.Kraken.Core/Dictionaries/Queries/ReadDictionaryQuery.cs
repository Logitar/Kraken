using Logitar.Kraken.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Kraken.Core.Dictionaries.Queries;

internal record ReadDictionaryQuery(Guid? Id) : Activity, IRequest<DictionaryModel>;

internal class ReadDictionaryQueryHandler : IRequestHandler<ReadDictionaryQuery, DictionaryModel?>
{
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public ReadDictionaryQueryHandler(IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<DictionaryModel?> Handle(ReadDictionaryQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, DictionaryModel> dictionaries = new(capacity: 2);

    if (query.Id.HasValue)
    {
      DictionaryModel? dictionary = await _dictionaryQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    //if (!string.IsNullOrWhiteSpace(query.Locale))
    //{
    //  DictionaryModel? dictionary = await _dictionaryQuerier.ReadAsync(query.Locale, cancellationToken);
    //  if (dictionary != null)
    //  {
    //    dictionaries[dictionary.Id] = dictionary;
    //  }
    //} // TODO(fpion): implement

    if (dictionaries.Count > 1)
    {
      throw TooManyResultsException<DictionaryModel>.ExpectedSingle(dictionaries.Count);
    }

    return dictionaries.Values.SingleOrDefault();
  }
}
