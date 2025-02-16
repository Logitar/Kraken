using Logitar.Kraken.Core.Passwords;

namespace Logitar.Kraken.Infrastructure.Passwords;

public interface IPasswordStrategy
{
  string Key { get; }

  Password Decode(string password);
  Password Hash(string password);
}
