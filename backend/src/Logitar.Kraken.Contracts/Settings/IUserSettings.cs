namespace Logitar.Kraken.Contracts.Settings;

public interface IUserSettings
{
  IUniqueNameSettings UniqueName { get; }
  IPasswordSettings Password { get; }
  bool RequireUniqueEmail { get; }
  bool RequireConfirmedAccount { get; } // TODO(fpion): this is not used.
}
