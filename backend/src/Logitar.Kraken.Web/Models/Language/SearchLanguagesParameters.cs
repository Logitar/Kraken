using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Web.Models.Search;

namespace Logitar.Kraken.Web.Models.Language;

public record SearchLanguagesParameters : SearchParameters
{
  public SearchLanguagesPayload ToPayload()
  {
    SearchLanguagesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out LanguageSort field))
      {
        payload.Sort.Add(new LanguageSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
