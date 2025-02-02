using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings;

public interface IUserSettings
{
  IUniqueNameSettings UniqueName { get; }
  IPasswordSettings Password { get; }
  public bool RequireUniqueEmail { get; }
  public bool RequireConfirmedAccount { get; }
}
