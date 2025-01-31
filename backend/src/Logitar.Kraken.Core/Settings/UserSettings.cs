using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings;

public record UserSettings(IUniqueNameSettings UniqueNameSettings, IPasswordSettings PasswordSettings) : IUserSettings;
