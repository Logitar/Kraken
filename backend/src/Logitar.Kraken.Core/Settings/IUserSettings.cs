using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings;

public interface IUserSettings
{
  IUniqueNameSettings UniqueNameSettings { get; }
  IPasswordSettings PasswordSettings { get; }
}
