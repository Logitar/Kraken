using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings;

public record UserSettings(
  IUniqueNameSettings UniqueNameSettings,
  IPasswordSettings PasswordSettings,
  bool RequireUniqueEmail,
  bool RequireConfirmedAccount) : IUserSettings;
