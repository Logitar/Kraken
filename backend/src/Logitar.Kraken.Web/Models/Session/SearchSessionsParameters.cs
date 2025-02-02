using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Models.Session;

public record SearchSessionsParameters : SearchParameters
{
  [FromQuery(Name = "user")]
  public Guid? UserId { get; set; }

  [FromQuery(Name = "active")]
  public bool? IsActive { get; set; }

  [FromQuery(Name = "persistent")]
  public bool? IsPersistent { get; set; }

  public SearchSessionsPayload ToPayload()
  {
    SearchSessionsPayload payload = new()
    {
      UserId = UserId,
      IsActive = IsActive,
      IsPersistent = IsPersistent
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out SessionSort field))
      {
        payload.Sort.Add(new SessionSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
