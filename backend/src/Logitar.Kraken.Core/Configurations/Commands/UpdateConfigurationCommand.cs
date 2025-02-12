using FluentValidation;
using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Core.Caching;
using Logitar.Kraken.Core.Configurations.Validators;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using MediatR;

namespace Logitar.Kraken.Core.Configurations.Commands;

public record UpdateConfigurationCommand(UpdateConfigurationPayload Payload) : IRequest<ConfigurationModel>;

internal class UpdateConfigurationCommandHandler : IRequestHandler<UpdateConfigurationCommand, ConfigurationModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly ISecretHelper _secretHelper;

  public UpdateConfigurationCommandHandler(
    IApplicationContext applicationContext,
    ICacheService cacheService,
    IConfigurationRepository configurationRepository,
    ISecretHelper secretHelper)
  {
    _applicationContext = applicationContext;
    _cacheService = cacheService;
    _configurationRepository = configurationRepository;
    _secretHelper = secretHelper;
  }

  public async Task<ConfigurationModel> Handle(UpdateConfigurationCommand command, CancellationToken cancellationToken)
  {
    UpdateConfigurationPayload payload = command.Payload;
    new UpdateConfigurationValidator().ValidateAndThrow(payload);

    Configuration configuration = await _configurationRepository.LoadAsync(cancellationToken) ?? throw new InvalidOperationException("The configuration was not loaded.");

    if (payload.Secret != null)
    {
      configuration.Secret = string.IsNullOrWhiteSpace(payload.Secret) ? _secretHelper.Generate() : _secretHelper.Encrypt(payload.Secret);
    }

    if (payload.UniqueNameSettings != null)
    {
      configuration.UniqueNameSettings = new UniqueNameSettings(payload.UniqueNameSettings);
    }
    if (payload.PasswordSettings != null)
    {
      configuration.PasswordSettings = new PasswordSettings(payload.PasswordSettings);
    }

    configuration.Update(_applicationContext.ActorId);
    await _configurationRepository.SaveAsync(configuration, cancellationToken);

    return _cacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
  }
}
