﻿using System.Collections.Immutable;

namespace Logitar.Kraken.Core.Users;

public record CountrySettings
{
  public string? PostalCode { get; init; }
  public ImmutableHashSet<string>? Regions { get; init; }
}
