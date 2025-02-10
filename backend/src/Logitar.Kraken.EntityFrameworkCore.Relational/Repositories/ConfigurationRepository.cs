using Logitar.EventSourcing;
using Logitar.Kraken.Core.Configurations;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class ConfigurationRepository : Repository, IConfigurationRepository
{
  public ConfigurationRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Configuration?> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync(version: null, cancellationToken);
  }
  public async Task<Configuration?> LoadAsync(long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Configuration>(new ConfigurationId().StreamId, version, cancellationToken);
  }

  public async Task SaveAsync(Configuration configuration, CancellationToken cancellationToken)
  {
    await base.SaveAsync(configuration, cancellationToken);

    // TODO(fpion): encache
  }
}
