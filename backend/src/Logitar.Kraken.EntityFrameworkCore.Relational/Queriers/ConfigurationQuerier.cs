using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Configurations;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class ConfigurationQuerier : IConfigurationQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ConfigurationEntity> _configurations;

  public ConfigurationQuerier(IActorService actorService, KrakenContext context)
  {
    _actorService = actorService;
    _configurations = context.Configurations;
  }

  public async Task<ConfigurationModel?> ReadAsync(CancellationToken cancellationToken)
  {
    ConfigurationEntity[] configurations = await _configurations.AsNoTracking().ToArrayAsync(cancellationToken);
    if (configurations.Length < 1)
    {
      return null;
    }

    List<ActorId> actorIds = new(capacity: configurations.Length * 2);
    foreach (ConfigurationEntity configuration in configurations)
    {
      if (configuration.CreatedBy != null)
      {
        actorIds.Add(new ActorId(configuration.CreatedBy));
      }
      if (configuration.UpdatedBy != null)
      {
        actorIds.Add(new ActorId(configuration.UpdatedBy));
      }
    }
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return mapper.ToConfiguration(configurations);
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
