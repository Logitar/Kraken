using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Caching;
using Logitar.Kraken.Core.Configurations.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Handlers;

internal class ConfigurationEvents : INotificationHandler<ConfigurationInitialized>, INotificationHandler<ConfigurationUpdated>
{
  private readonly IActorService _actorService;
  private readonly ICacheService _cacheService;
  private readonly KrakenContext _context;
  private readonly ILogger<ConfigurationEvents> _logger;

  public ConfigurationEvents(IActorService actorService, ICacheService cacheService, KrakenContext context, ILogger<ConfigurationEvents> logger)
  {
    _actorService = actorService;
    _cacheService = cacheService;
    _context = context;
    _logger = logger;
  }

  public async Task Handle(ConfigurationInitialized @event, CancellationToken cancellationToken)
  {
    Dictionary<string, ConfigurationEntity> configurations = new(capacity: 1 + 1 + 7);
    HandleChange(configurations, ConfigurationKeys.Secret, @event.Secret, @event);
    HandleChange(configurations, ConfigurationKeys.AllowedUniqueNameCharacters, @event.UniqueNameSettings.AllowedCharacters, @event);
    HandleChange(configurations, ConfigurationKeys.PasswordHashingStrategy, @event.PasswordSettings.HashingStrategy, @event);
    HandleChange(configurations, ConfigurationKeys.PasswordsRequireDigit, @event.PasswordSettings.RequireDigit, @event);
    HandleChange(configurations, ConfigurationKeys.PasswordsRequireLowercase, @event.PasswordSettings.RequireLowercase, @event);
    HandleChange(configurations, ConfigurationKeys.PasswordsRequireNonAlphanumeric, @event.PasswordSettings.RequireNonAlphanumeric, @event);
    HandleChange(configurations, ConfigurationKeys.PasswordsRequireUppercase, @event.PasswordSettings.RequireUppercase, @event);
    HandleChange(configurations, ConfigurationKeys.RequiredPasswordLength, @event.PasswordSettings.RequiredLength, @event);
    HandleChange(configurations, ConfigurationKeys.RequiredPasswordUniqueChars, @event.PasswordSettings.RequiredUniqueChars, @event);

    await _context.SaveChangesAsync(cancellationToken);

    await RefreshCacheAsync(configurations.Values, cancellationToken);

    _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
  }

  public async Task Handle(ConfigurationUpdated @event, CancellationToken cancellationToken)
  {
    Dictionary<string, ConfigurationEntity> configurations = (await _context.Configurations.ToArrayAsync(cancellationToken))
      .ToDictionary(x => x.Key, x => x);
    if (@event.Secret != null)
    {
      HandleChange(configurations, ConfigurationKeys.Secret, @event.Secret, @event);
    }
    if (@event.UniqueNameSettings != null)
    {
      HandleChange(configurations, ConfigurationKeys.AllowedUniqueNameCharacters, @event.UniqueNameSettings.AllowedCharacters, @event);
    }
    if (@event.PasswordSettings != null)
    {
      HandleChange(configurations, ConfigurationKeys.PasswordHashingStrategy, @event.PasswordSettings.HashingStrategy, @event);
      HandleChange(configurations, ConfigurationKeys.PasswordsRequireDigit, @event.PasswordSettings.RequireDigit, @event);
      HandleChange(configurations, ConfigurationKeys.PasswordsRequireLowercase, @event.PasswordSettings.RequireLowercase, @event);
      HandleChange(configurations, ConfigurationKeys.PasswordsRequireNonAlphanumeric, @event.PasswordSettings.RequireNonAlphanumeric, @event);
      HandleChange(configurations, ConfigurationKeys.PasswordsRequireUppercase, @event.PasswordSettings.RequireUppercase, @event);
      HandleChange(configurations, ConfigurationKeys.RequiredPasswordLength, @event.PasswordSettings.RequiredLength, @event);
      HandleChange(configurations, ConfigurationKeys.RequiredPasswordUniqueChars, @event.PasswordSettings.RequiredUniqueChars, @event);
    }

    await _context.SaveChangesAsync(cancellationToken);

    await RefreshCacheAsync(configurations.Values, cancellationToken);

    _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
  }

  private void HandleChange(Dictionary<string, ConfigurationEntity> configurations, string key, object? value, DomainEvent @event)
  {
    string? stringValue = value?.ToString();
    if (configurations.TryGetValue(key, out ConfigurationEntity? configuration))
    {
      if (stringValue == null)
      {
        configurations.Remove(key);
        _context.Configurations.Remove(configuration);
      }
      else
      {
        configuration.Update(stringValue, @event);
      }
    }
    else if (stringValue != null)
    {
      configuration = new(key, stringValue, @event);
      configurations[key] = configuration;
      _context.Configurations.Add(configuration);
    }
  }

  private async Task RefreshCacheAsync(IReadOnlyCollection<ConfigurationEntity> configurations, CancellationToken cancellationToken)
  {
    List<ActorId> actorIds = new(capacity: configurations.Count * 2);
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

    _cacheService.Configuration = mapper.ToConfiguration(configurations);
    _logger.LogInformation("Configuration cache has been refreshed.");
  }
}
