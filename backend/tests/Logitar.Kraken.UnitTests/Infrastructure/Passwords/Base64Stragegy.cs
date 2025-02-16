﻿using Logitar.Kraken.Core.Passwords;

namespace Logitar.Kraken.Infrastructure.Passwords;

internal class Base64Stragegy : IPasswordStrategy
{
  public string Key => Base64Password.Key;

  public Password Decode(string password) => Base64Password.Decode(password);

  public Password Hash(string password) => new Base64Password(password);
}
