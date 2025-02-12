using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Web.Models.Search;

namespace Logitar.Kraken.Web.Models.Realm;

public record SearchRealmsParameters : SearchParameters
{
  public SearchRealmsPayload ToPayload()
  {
    SearchRealmsPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out RealmSort field))
      {
        payload.Sort.Add(new RealmSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
