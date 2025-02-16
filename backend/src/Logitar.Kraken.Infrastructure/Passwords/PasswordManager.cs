using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Passwords;
using Logitar.Security.Cryptography;

namespace Logitar.Kraken.Infrastructure.Passwords;

public class PasswordManager : IPasswordManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual Dictionary<string, IPasswordStrategy> Strategies { get; } = [];

  public PasswordManager(IApplicationContext applicationContext, IEnumerable<IPasswordStrategy> strategies)
  {
    ApplicationContext = applicationContext;

    foreach (IPasswordStrategy strategy in strategies)
    {
      Strategies[strategy.Key] = strategy;
    }
  }

  public virtual Password Decode(string password)
  {
    string key = password.Split(Password.Separator).First();
    return GetStrategy(key).Decode(password);
  }

  public virtual Password Generate(int length, out string password)
  {
    password = RandomStringGenerator.GetString(length);
    return Hash(password);
  }
  public virtual Password Generate(string characters, int length, out string password)
  {
    password = RandomStringGenerator.GetString(characters, length);
    return Hash(password);
  }

  public virtual Password GenerateBase64(int length, out string password)
  {
    password = RandomStringGenerator.GetBase64String(length, out _);
    return Hash(password);
  }

  public virtual Password Hash(string password) => Hash(settings: null, password);
  protected virtual Password Hash(IPasswordSettings? settings, string password)
  {
    settings ??= ApplicationContext.UserSettings.Password;
    return GetStrategy(settings.HashingStrategy).Hash(password);
  }

  public virtual void Validate(string password)
  {
    Validate(settings: null, password);
  }
  protected virtual void Validate(IPasswordSettings? settings, string password)
  {
    settings ??= ApplicationContext.UserSettings.Password;
    _ = new PasswordInput(settings, password);
  }

  public virtual Password ValidateAndHash(string password) => ValidateAndHash(settings: null, password);
  public virtual Password ValidateAndHash(IPasswordSettings? settings, string password)
  {
    settings ??= ApplicationContext.UserSettings.Password;
    Validate(settings, password);
    return Hash(settings, password);
  }

  protected virtual IPasswordStrategy GetStrategy(string key)
  {
    return Strategies.TryGetValue(key, out IPasswordStrategy? strategy) ? strategy : throw new PasswordStrategyNotSupportedException(key);
  }
}
