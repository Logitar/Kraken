using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Passwords;

namespace Logitar.Kraken.Infrastructure.Passwords;

internal class PasswordManager : IPasswordManager
{
  public Password Create(IPasswordSettings settings, string password)
  {
    throw new NotImplementedException();
  }

  public Password Decode(string value)
  {
    throw new NotImplementedException();
  }
}
