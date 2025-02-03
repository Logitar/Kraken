using Logitar.Kraken.Contracts.Dictionaries;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Models.Dictionary;

public record SearchDictionariesParameters : SearchParameters
{
  [FromQuery(Name = "empty")]
  public bool? IsEmpty { get; set; }

  public SearchDictionariesPayload ToPayload()
  {
    SearchDictionariesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out DictionarySort field))
      {
        payload.Sort.Add(new DictionarySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
