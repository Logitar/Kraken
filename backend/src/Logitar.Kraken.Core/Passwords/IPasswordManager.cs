using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Passwords;

public interface IPasswordManager
{
  Password ValidateAndCreate(IPasswordSettings settings, string password);
}
