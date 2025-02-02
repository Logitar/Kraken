using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings;

public record UserSettings(
  IUniqueNameSettings UniqueName,
  IPasswordSettings Password,
  bool RequireUniqueEmail,
  bool RequireConfirmedAccount) : IUserSettings;
