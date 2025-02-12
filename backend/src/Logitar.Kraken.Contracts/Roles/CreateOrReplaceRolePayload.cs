﻿namespace Logitar.Kraken.Contracts.Roles;

public record CreateOrReplaceRolePayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<CustomAttributeModel> CustomAttributes { get; set; } = [];
}
