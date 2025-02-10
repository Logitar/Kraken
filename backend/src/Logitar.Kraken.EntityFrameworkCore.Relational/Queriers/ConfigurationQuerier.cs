using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Configurations;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class ConfigurationQuerier : IConfigurationQuerier
{
  private readonly IActorService _actorService;

  public ConfigurationQuerier(IActorService actorService)
  {
    _actorService = actorService;
  }

  public Task<ConfigurationModel?> ReadAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public async Task<ConfigurationModel> ReadAsync(Configuration configuration, CancellationToken cancellationToken)
  {
    List<ActorId> actorIds = new(capacity: 2);
    if (configuration.CreatedBy.HasValue)
    {
      actorIds.Add(configuration.CreatedBy.Value);
    }
    if (configuration.UpdatedBy.HasValue)
    {
      actorIds.Add(configuration.UpdatedBy.Value);
    }
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return mapper.ToConfiguration(configuration);
  }
}
