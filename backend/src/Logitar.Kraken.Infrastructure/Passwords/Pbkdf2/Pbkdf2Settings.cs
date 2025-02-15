using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace Logitar.Kraken.Infrastructure.Passwords.Pbkdf2;

internal record Pbkdf2Settings
{
  public KeyDerivationPrf Algorithm { get; set; } = KeyDerivationPrf.HMACSHA256;
  public int Iterations { get; set; } = 600000;
  public int SaltLength { get; set; } = 256 / 8;
  public int? HashLength { get; set; }

  public static Pbkdf2Settings Initialize(IConfiguration configuration)
  {
    Pbkdf2Settings settings = configuration.GetSection("Passwords:Pbkdf2").Get<Pbkdf2Settings>() ?? new();

    settings.Algorithm = EnvironmentHelper.GetEnum("PASSWORDS_PBKDF2_ALGORITHM", settings.Algorithm);
    settings.Iterations = EnvironmentHelper.GetInt32("PASSWORDS_PBKDF2_ITERATIONS", settings.Iterations);
    settings.SaltLength = EnvironmentHelper.GetInt32("PASSWORDS_PBKDF2_SALT_LENGTH", settings.SaltLength);
    settings.HashLength = EnvironmentHelper.TryGetInt32("PASSWORDS_PBKDF2_HASH_LENGTH", settings.HashLength);

    return settings;
  }
}
