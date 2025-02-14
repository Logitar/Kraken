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

    string? algorithmValue = Environment.GetEnvironmentVariable("PASSWORDS_PBKDF2_ALGORITHM");
    if (!string.IsNullOrWhiteSpace(algorithmValue) && Enum.TryParse(algorithmValue.Trim(), out KeyDerivationPrf algorithm))
    {
      settings.Algorithm = algorithm;
    }

    string? iterationsValue = Environment.GetEnvironmentVariable("PASSWORDS_PBKDF2_ITERATIONS");
    if (!string.IsNullOrWhiteSpace(iterationsValue) && int.TryParse(iterationsValue.Trim(), out int iterations))
    {
      settings.Iterations = iterations;
    }

    string? saltLengthValue = Environment.GetEnvironmentVariable("PASSWORDS_PBKDF2_SALT_LENGTH");
    if (!string.IsNullOrWhiteSpace(saltLengthValue) && int.TryParse(saltLengthValue.Trim(), out int saltLength))
    {
      settings.SaltLength = saltLength;
    }

    string? hashLengthValue = Environment.GetEnvironmentVariable("PASSWORDS_PBKDF2_HASH_LENGTH");
    if (!string.IsNullOrWhiteSpace(hashLengthValue) && int.TryParse(hashLengthValue.Trim(), out int hashLength))
    {
      settings.HashLength = hashLength;
    }

    return settings;
  }
}
