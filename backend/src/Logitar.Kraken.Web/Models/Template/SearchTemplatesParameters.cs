using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Templates;
using Logitar.Kraken.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Models.Template;

public record SearchTemplatesParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public string? ContentType { get; set; }

  public SearchTemplatesPayload ToPayload()
  {
    SearchTemplatesPayload payload = new()
    {
      ContentType = ContentType
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out TemplateSort field))
      {
        payload.Sort.Add(new TemplateSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
