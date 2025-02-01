﻿using FluentValidation;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Users.Validators;

namespace Logitar.Kraken.Core.Users;

public record Email : Contact, IEmail
{
  public const int MaximumLength = byte.MaxValue;

  public string Address { get; }

  public Email(string address, bool isVerified = false) : base(isVerified)
  {
    Address = address.Trim();
    new EmailValidator().ValidateAndThrow(this);
  }

  public override string ToString() => Address;
}
