﻿using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Kraken.Web.Models.ApiKey;

public record SearchApiKeysParameters : SearchParameters
{
  [FromQuery(Name = "role")]
  public Guid? RoleId { get; set; }

  [FromQuery(Name = "expired")]
  public bool? IsExpired { get; set; }

  [FromQuery(Name = "moment")]
  public DateTime? Moment { get; set; }

  public SearchApiKeysPayload ToPayload()
  {
    SearchApiKeysPayload payload = new();
    if (IsExpired.HasValue)
    {
      payload.Status = new ApiKeyStatus(IsExpired.Value, Moment);
    }
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ApiKeySort field))
      {
        payload.Sort.Add(new ApiKeySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
