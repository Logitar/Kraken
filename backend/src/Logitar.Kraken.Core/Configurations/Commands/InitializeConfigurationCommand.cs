using Logitar.EventSourcing;
using Logitar.Kraken.Core.Caching;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Configurations.Commands;

public record InitializeConfigurationCommand(string DefaultLocale, string UniqueName, string Password) : Activity, IRequest;

internal class InitializeConfigurationCommandHandler : IRequestHandler<InitializeConfigurationCommand>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly ILanguageManager _languageManager;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;

  public InitializeConfigurationCommandHandler(
    ICacheService cacheService,
    IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository,
    ILanguageManager languageManager,
    IPasswordManager passwordManager,
    IUserManager userManager)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
    _languageManager = languageManager;
    _passwordManager = passwordManager;
    _userManager = userManager;
  }

  public async Task Handle(InitializeConfigurationCommand command, CancellationToken cancellationToken)
  {
    Configuration? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration == null)
    {
      UserId userId = UserId.NewId(realmId: null);
      ActorId actorId = new(userId.Value);

      configuration = Configuration.Initialize(actorId);

      Locale locale = new(command.DefaultLocale);
      Language language = new(locale, isDefault: true, actorId);

      UniqueName uniqueName = new(configuration.UniqueNameSettings, command.UniqueName);
      User user = new(uniqueName, actorId, userId);

      Password password = _passwordManager.ValidateAndCreate(configuration.PasswordSettings, command.Password);
      user.SetPassword(password, actorId);

      await _configurationRepository.SaveAsync(configuration, cancellationToken); // NOTE(fpion): this should cache the configuration.
      await _languageManager.SaveAsync(language, cancellationToken);
      await _userManager.SaveAsync(configuration.UserSettings, user, actorId, cancellationToken);
    }
    else
    {
      _cacheService.Configuration = await _configurationQuerier.ReadAsync(configuration, cancellationToken);
    }
  }
}
