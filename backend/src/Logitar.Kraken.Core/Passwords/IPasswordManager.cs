using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Passwords;

public interface IPasswordManager
{
  Password Create(IPasswordSettings settings, string password);
  Password Decode(string value);
  Password GenerateBase64(int length, out string password);
}
