﻿namespace Logitar.Kraken.Contracts.Users;

public interface IPhone : IContact
{
  string? CountryCode { get; }
  string Number { get; }
  string? Extension { get; }
}
