using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Contracts.Localization;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Configurations;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Kraken.EntityFrameworkCore.Relational;

internal class Mapper
{
  private readonly Dictionary<ActorId, ActorModel> _actors = [];
  private readonly ActorModel _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<ActorModel> actors)
  {
    foreach (var actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public ConfigurationModel ToConfiguration(Configuration source)
  {
    ConfigurationModel destination = new()
    {
      UniqueNameSettings = new UniqueNameSettingsModel(source.UniqueNameSettings),
      PasswordSettings = new PasswordSettingsModel(source.PasswordSettings)
    };

    MapAggregate(source, destination);

    return destination;
  }
  public ConfigurationModel ToConfiguration(IEnumerable<ConfigurationEntity> configurations)
  {
    if (!configurations.Any())
    {
      throw new ArgumentException("At least one configuration must be provided.", nameof(configurations));
    }

    ConfigurationModel destination = new()
    {
      CreatedOn = DateTime.MaxValue,
      UpdatedOn = DateTime.MinValue
    };

    foreach (ConfigurationEntity configuration in configurations)
    {
      if (configuration.Version > destination.Version)
      {
        destination.Version = configuration.Version;
      }
      if (configuration.CreatedOn < destination.CreatedOn)
      {
        destination.CreatedBy = TryGetActor(configuration.CreatedBy) ?? _system;
        destination.CreatedOn = configuration.CreatedOn.AsUniversalTime();
      }
      if (configuration.UpdatedOn > destination.UpdatedOn)
      {
        destination.UpdatedBy = TryGetActor(configuration.UpdatedBy) ?? _system;
        destination.UpdatedOn = configuration.UpdatedOn.AsUniversalTime();
      }

      switch (configuration.Key)
      {
        case ConfigurationKeys.AllowedUniqueNameCharacters:
          destination.UniqueNameSettings.AllowedCharacters = configuration.Value;
          break;
        case ConfigurationKeys.PasswordHashingStrategy:
          destination.PasswordSettings.HashingStrategy = configuration.Value;
          break;
        case ConfigurationKeys.PasswordsRequireDigit:
          destination.PasswordSettings.RequireDigit = bool.Parse(configuration.Value);
          break;
        case ConfigurationKeys.PasswordsRequireLowercase:
          destination.PasswordSettings.RequireLowercase = bool.Parse(configuration.Value);
          break;
        case ConfigurationKeys.PasswordsRequireNonAlphanumeric:
          destination.PasswordSettings.RequireNonAlphanumeric = bool.Parse(configuration.Value);
          break;
        case ConfigurationKeys.PasswordsRequireUppercase:
          destination.PasswordSettings.RequireUppercase = bool.Parse(configuration.Value);
          break;
        case ConfigurationKeys.RequiredPasswordLength:
          destination.PasswordSettings.RequiredLength = int.Parse(configuration.Value);
          break;
        case ConfigurationKeys.RequiredPasswordUniqueChars:
          destination.PasswordSettings.RequiredUniqueChars = int.Parse(configuration.Value);
          break;
      }
    }

    return destination;
  }

  public LanguageModel ToLanguage(LanguageEntity source, RealmModel? realm)
  {
    LanguageModel destination = new()
    {
      Id = source.Id,
      IsDefault = source.IsDefault,
      Locale = source.GetLocale(),
      Realm = realm
    };

    MapAggregate(source, destination);

    return destination;
  }

  public RealmModel ToRealm(RealmEntity source)
  {
    RealmModel destination = new()
    {
      Id = source.Id,
      UniqueSlug = source.UniqueSlug,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Url = source.Url,
      UniqueNameSettings = source.GetUniqueNameSettings(),
      PasswordSettings = source.GetPasswordSettings(),
      RequireUniqueEmail = source.RequireUniqueEmail,
      RequireConfirmedAccount = source.RequireConfirmedAccount
    };

    foreach (var customAttribute in source.GetCustomAttributes())
    {
      destination.CustomAttributes.Add(new CustomAttributeModel(customAttribute));
    }

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateRoot source, AggregateModel destination)
  {
    destination.Version = source.Version;
    destination.CreatedBy = TryGetActor(source.CreatedBy) ?? _system;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = TryGetActor(source.UpdatedBy) ?? _system;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }
  private void MapAggregate(AggregateEntity source, AggregateModel destination)
  {
    destination.Version = source.Version;
    destination.CreatedBy = TryGetActor(source.CreatedBy) ?? _system;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = TryGetActor(source.UpdatedBy) ?? _system;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private ActorModel? TryGetActor(string? id) => id == null ? null : TryGetActor(new ActorId(id));
  private ActorModel? TryGetActor(ActorId? id)
  {
    ActorModel? actor = null;
    if (id.HasValue)
    {
      _ = _actors.TryGetValue(id.Value, out actor);
    }
    return actor;
  }
}
