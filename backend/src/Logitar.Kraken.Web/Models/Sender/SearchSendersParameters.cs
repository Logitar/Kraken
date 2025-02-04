using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Models.Sender;

public record SearchSendersParameters : SearchParameters
{
  [FromQuery(Name = "provider")]
  public SenderProvider? Provider { get; set; }

  public SearchSendersPayload ToPayload()
  {
    SearchSendersPayload payload = new()
    {
      Provider = Provider
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out SenderSort field))
      {
        payload.Sort.Add(new SenderSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
