using Logitar.Kraken.Core.Passwords;

namespace Logitar.Kraken.Infrastructure.Passwords.Pbkdf2;

internal class Pbkdf2Strategy : IPasswordStrategy
{
  private readonly Pbkdf2Settings _settings;

  public string Key => Pbkdf2Password.Key;

  public Pbkdf2Strategy(Pbkdf2Settings settings)
  {
    _settings = settings;
  }

  public Password Decode(string password) => Pbkdf2Password.Decode(password);

  public Password Hash(string password) => new Pbkdf2Password(password, _settings.Algorithm, _settings.Iterations, _settings.SaltLength, _settings.HashLength);
}
