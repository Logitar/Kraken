using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings;

public interface IRoleSettings
{
  IUniqueNameSettings UniqueName { get; }
}
