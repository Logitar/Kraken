using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Web.Models.Search;

namespace Logitar.Kraken.Web.Models.Role;

public record SearchRolesParameters : SearchParameters
{
  public SearchRolesPayload ToPayload()
  {
    SearchRolesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out RoleSort field))
      {
        payload.Sort.Add(new RoleSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
