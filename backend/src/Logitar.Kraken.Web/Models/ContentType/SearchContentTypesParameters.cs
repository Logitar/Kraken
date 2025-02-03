using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Models.ContentType;

public record SearchContentTypesParameters : SearchParameters
{
  [FromQuery(Name = "invariant")]
  public bool? IsInvariant { get; set; }

  public SearchContentTypesPayload ToPayload()
  {
    SearchContentTypesPayload payload = new()
    {
      IsInvariant = IsInvariant
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ContentTypeSort field))
      {
        payload.Sort.Add(new ContentTypeSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
