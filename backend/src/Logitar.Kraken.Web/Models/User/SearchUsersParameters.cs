using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Models.User;

public record SearchUsersParameters : SearchParameters
{
  [FromQuery(Name = "authenticated")]
  public bool? HasAuthenticated { get; set; }

  [FromQuery(Name = "password")]
  public bool? HasPassword { get; set; }

  [FromQuery(Name = "disabled")]
  public bool? IsDisabled { get; set; }

  [FromQuery(Name = "confirmed")]
  public bool? IsConfirmed { get; set; }

  [FromQuery(Name = "role")]
  public Guid? RoleId { get; set; }

  public SearchUsersPayload ToPayload()
  {
    SearchUsersPayload payload = new()
    {
      HasAuthenticated = HasAuthenticated,
      HasPassword = HasPassword,
      IsDisabled = IsDisabled,
      IsConfirmed = IsConfirmed,
      RoleId = RoleId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out UserSort field))
      {
        payload.Sort.Add(new UserSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
