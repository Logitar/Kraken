namespace Logitar.Kraken.Core.Configurations;

public interface IConfigurationRepository
{
  Task<Configuration?> LoadAsync(CancellationToken cancellationToken = default);
  Task<Configuration?> LoadAsync(long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Configuration configuration, CancellationToken cancellationToken = default);
}
