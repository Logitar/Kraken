using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings;

public record RoleSettings(IUniqueNameSettings UniqueName) : IRoleSettings;
