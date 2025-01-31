﻿using FluentValidation;
using Logitar.Security.Cryptography;

namespace Logitar.Kraken.Core.Tokens;

public record JwtSecret
{
  public const int MinimumLength = 256 / 8;
  public const int MaximumLength = 512 / 8;

  public string Value { get; }

  public JwtSecret(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static JwtSecret Generate() => Generate(MinimumLength);
  public static JwtSecret Generate(int length) => new(RandomStringGenerator.GetString(length));

  public override string ToString() => Value;

  private class Validator : AbstractValidator<JwtSecret>
  {
    public Validator()
    {
      RuleFor(x => x.Value).JwtSecret();
    }
  }
}
