using FluentValidation;
using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Core.Caching;
using Logitar.Kraken.Core.Configurations.Validators;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using MediatR;

namespace Logitar.Kraken.Core.Configurations.Commands;

public record ReplaceConfigurationCommand(ReplaceConfigurationPayload Payload, long? Version) : IRequest<ConfigurationModel>;

internal class ReplaceConfigurationCommandHandler : IRequestHandler<ReplaceConfigurationCommand, ConfigurationModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;

  public ReplaceConfigurationCommandHandler(IApplicationContext applicationContext, ICacheService cacheService, IConfigurationRepository configurationRepository)
  {
    _applicationContext = applicationContext;
    _cacheService = cacheService;
    _configurationRepository = configurationRepository;
  }

  public async Task<ConfigurationModel?> Handle(ReplaceConfigurationCommand command, CancellationToken cancellationToken)
  {
    ReplaceConfigurationPayload payload = command.Payload;
    new ReplaceConfigurationValidator().ValidateAndThrow(payload);

    Configuration configuration = await _configurationRepository.LoadAsync(cancellationToken)
      ?? throw new InvalidOperationException("The configuration has not been initialized yet.");

    Configuration reference = (command.Version.HasValue
      ? await _configurationRepository.LoadAsync(command.Version.Value, cancellationToken)
      : null) ?? configuration;

    JwtSecret secret = JwtSecret.CreateOrGenerate(payload.Secret);
    if (reference.Secret != secret)
    {
      configuration.Secret = secret;
    }

    UniqueNameSettings uniqueNameSettings = new(payload.UniqueNameSettings);
    if (reference.UniqueNameSettings != uniqueNameSettings)
    {
      configuration.UniqueNameSettings = uniqueNameSettings;
    }
    PasswordSettings passwordSettings = new(payload.PasswordSettings);
    if (reference.PasswordSettings != passwordSettings)
    {
      configuration.PasswordSettings = passwordSettings;
    }

    LoggingSettings loggingSettings = new(payload.LoggingSettings);
    if (reference.LoggingSettings != loggingSettings)
    {
      configuration.LoggingSettings = loggingSettings;
    }

    configuration.Update(_applicationContext.ActorId);
    await _configurationRepository.SaveAsync(configuration, cancellationToken);

    return _cacheService.Configuration ?? throw new InvalidOperationException("The configuration should be in the cache.");
  }
}
