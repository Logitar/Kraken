using Logitar.Kraken.Contracts.Configurations;

namespace Logitar.Kraken.Core.Configurations;

public interface IConfigurationQuerier
{
  Task<ConfigurationModel> ReadAsync(CancellationToken cancellationToken = default);
  Task<ConfigurationModel> ReadAsync(Configuration configuration, CancellationToken cancellationToken = default);
}
