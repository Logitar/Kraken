using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Logitar.Kraken.Infrastructure.Passwords.Pbkdf2;

internal record Pbkdf2Settings
{
  public const string SectionKey = "Identity:Passwords:Pbkdf2";

  public KeyDerivationPrf Algorithm { get; set; } = KeyDerivationPrf.HMACSHA256;
  public int Iterations { get; set; } = 600000;
  public int SaltLength { get; set; } = 256 / 8;
  public int? HashLength { get; set; }

  // TODO(fpion): Initialize
}
