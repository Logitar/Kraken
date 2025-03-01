﻿using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Roles;

namespace Logitar.Kraken.Contracts.ApiKeys;

public class ApiKeyModel : AggregateModel
{
  public string? XApiKey { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];
  public List<RoleModel> Roles { get; set; } = [];

  public RealmModel? Realm { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
