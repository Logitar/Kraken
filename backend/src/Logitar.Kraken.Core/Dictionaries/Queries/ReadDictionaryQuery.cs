using Logitar.Kraken.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Kraken.Core.Dictionaries.Queries;

/// <exception cref="TooManyResultsException{T}"></exception>
public record ReadDictionaryQuery(Guid? Id, string? Language) : Activity, IRequest<DictionaryModel>;

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

    if (!string.IsNullOrWhiteSpace(query.Language))
    {
      DictionaryModel? dictionary = await _dictionaryQuerier.ReadAsync(query.Language, cancellationToken);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (dictionaries.Count > 1)
    {
      throw TooManyResultsException<DictionaryModel>.ExpectedSingle(dictionaries.Count);
    }

    return dictionaries.Values.SingleOrDefault();
  }
}
