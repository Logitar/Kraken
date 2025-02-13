using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings;

public record RoleSettings : IRoleSettings
{
  public IUniqueNameSettings UniqueName { get; }

  public RoleSettings(UniqueNameSettings uniqueName)
  {
    UniqueName = uniqueName;
  }
}
