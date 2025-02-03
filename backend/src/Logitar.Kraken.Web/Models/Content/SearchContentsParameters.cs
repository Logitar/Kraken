using Logitar.Kraken.Contracts.Contents;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Models.Content;

public record SearchContentsParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public Guid? ContentTypeId { get; set; }

  [FromQuery(Name = "language")]
  public Guid? LanguageId { get; set; }

  public SearchContentsPayload ToPayload()
  {
    SearchContentsPayload payload = new()
    {
      ContentTypeId = ContentTypeId,
      LanguageId = LanguageId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ContentSort field))
      {
        payload.Sort.Add(new ContentSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
