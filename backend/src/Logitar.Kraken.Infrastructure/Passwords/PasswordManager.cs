using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Passwords;

namespace Logitar.Kraken.Infrastructure.Passwords;

internal class PasswordManager : IPasswordManager // TODO(fpion): implement
{
  public Password Create(IPasswordSettings settings, string password)
  {
    throw new NotImplementedException();
  }

  public Password Decode(string value)
  {
    throw new NotImplementedException();
  }

  public Password GenerateBase64(int length, out string password)
  {
    throw new NotImplementedException();
  }
}
