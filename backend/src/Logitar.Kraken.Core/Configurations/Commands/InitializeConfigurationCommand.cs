using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Kraken.Core.Caching;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Tokens;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Configurations.Commands;

/// <exception cref="ValidationException"></exception>
public record InitializeConfigurationCommand(string DefaultLocale, string UniqueName, string Password) : Activity, IRequest
{
  public override IActivity Anonymize()
  {
    InitializeConfigurationCommand anonymized = new(DefaultLocale, UniqueName, Password.Mask());
    return anonymized;
  }
}

internal class InitializeConfigurationCommandHandler : IRequestHandler<InitializeConfigurationCommand>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly ILanguageManager _languageManager;
  private readonly IPasswordManager _passwordManager;
  private readonly ISecretHelper _secretHelper;
  private readonly IUserManager _userManager;

  public InitializeConfigurationCommandHandler(
    ICacheService cacheService,
    IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository,
    ILanguageManager languageManager,
    IPasswordManager passwordManager,
    ISecretHelper secretHelper,
    IUserManager userManager)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
    _languageManager = languageManager;
    _passwordManager = passwordManager;
    _secretHelper = secretHelper;
    _userManager = userManager;
  }

  public async Task Handle(InitializeConfigurationCommand command, CancellationToken cancellationToken)
  {
    Configuration? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration == null)
    {
      UserId userId = UserId.NewId();
      ActorId actorId = new(userId.Value);

      Secret secret = _secretHelper.Generate();
      configuration = Configuration.Initialize(secret, actorId);

      Locale locale = new(command.DefaultLocale);
      Language language = new(locale, isDefault: true, actorId);

      UniqueName uniqueName = new(configuration.UniqueNameSettings, command.UniqueName);
      Password password = _passwordManager.ValidateAndHash(configuration.PasswordSettings, command.Password);
      User user = new(uniqueName, password, actorId, userId);

      await _configurationRepository.SaveAsync(configuration, cancellationToken); // NOTE(fpion): this should cache the configuration.
      await _languageManager.SaveAsync(language, cancellationToken);
      await _userManager.SaveAsync(user, cancellationToken);
    }
    else
    {
      _cacheService.Configuration = await _configurationQuerier.ReadAsync(configuration, cancellationToken);
    }
  }
}
