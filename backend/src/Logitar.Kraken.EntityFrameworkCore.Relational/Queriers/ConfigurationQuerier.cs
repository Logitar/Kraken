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

  public async Task<ConfigurationModel> ReadAsync(CancellationToken cancellationToken)
  {
    ConfigurationEntity[] configurations = await _configurations.AsNoTracking()
      .OrderBy(x => x.CreatedOn)
      .ToArrayAsync(cancellationToken);

    ConfigurationModel configuration = new();
    if (configurations.Length > 0)
    {
      foreach (ConfigurationEntity entity in configurations)
      {
        configuration.Version = Math.Max(configuration.Version, entity.Version);

        switch (entity.Key)
        {
          case ConfigurationKeys.AllowedUniqueNameCharacters:
            configuration.UniqueNameSettings.AllowedCharacters = entity.Value;
            break;
          case ConfigurationKeys.PasswordHashingStrategy:
            configuration.PasswordSettings.HashingStrategy = entity.Value;
            break;
          case ConfigurationKeys.PasswordsRequireDigit:
            configuration.PasswordSettings.RequireDigit = bool.Parse(entity.Value);
            break;
          case ConfigurationKeys.PasswordsRequireLowercase:
            configuration.PasswordSettings.RequireLowercase = bool.Parse(entity.Value);
            break;
          case ConfigurationKeys.PasswordsRequireUppercase:
            configuration.PasswordSettings.RequireUppercase = bool.Parse(entity.Value);
            break;
          case ConfigurationKeys.PasswordsRequireNonAlphanumeric:
            configuration.PasswordSettings.RequireNonAlphanumeric = bool.Parse(entity.Value);
            break;
          case ConfigurationKeys.RequiredPasswordLength:
            configuration.PasswordSettings.RequiredLength = int.Parse(entity.Value);
            break;
          case ConfigurationKeys.RequiredPasswordUniqueChars:
            configuration.PasswordSettings.RequiredUniqueChars = int.Parse(entity.Value);
            break;
        }
      }

      List<ActorId> actorIds = new(capacity: 2);

      ConfigurationEntity earliest = configurations.First();
      if (earliest.CreatedBy != null)
      {
        actorIds.Add(new ActorId(earliest.CreatedBy));
      }
      configuration.CreatedOn = earliest.CreatedOn.AsUniversalTime();

      ConfigurationEntity latest = configurations.OrderByDescending(x => x.UpdatedOn).First();
      if (latest.UpdatedBy != null)
      {
        actorIds.Add(new ActorId(latest.UpdatedBy));
      }
      configuration.UpdatedOn = latest.UpdatedOn.AsUniversalTime();

      Dictionary<string, ActorModel> actors = (await _actorService.FindAsync(actorIds, cancellationToken))
        .ToDictionary(x => new ActorId(x.Id).Value, x => x);
      ActorModel system = new();
      if (earliest.CreatedBy != null)
      {
        configuration.CreatedBy = actors.TryGetValue(earliest.CreatedBy, out ActorModel? actor) ? actor : system;
      }
      if (latest.UpdatedBy != null)
      {
        configuration.UpdatedBy = actors.TryGetValue(latest.UpdatedBy, out ActorModel? actor) ? actor : system;
      }
    }

    return configuration;
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
