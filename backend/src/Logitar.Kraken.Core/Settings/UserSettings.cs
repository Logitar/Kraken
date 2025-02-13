using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings;

public record UserSettings : IUserSettings
{
  public IUniqueNameSettings UniqueName { get; }
  public IPasswordSettings Password { get; }
  public bool RequireUniqueEmail { get; }
  public bool RequireConfirmedAccount { get; }

  public UserSettings(UniqueNameSettings uniqueName, PasswordSettings password, bool requireUniqueEmail, bool requireConfirmedAccount)
  {
    UniqueName = uniqueName;
    Password = password;
    RequireUniqueEmail = requireUniqueEmail;
    RequireConfirmedAccount = requireConfirmedAccount;
  }
}
