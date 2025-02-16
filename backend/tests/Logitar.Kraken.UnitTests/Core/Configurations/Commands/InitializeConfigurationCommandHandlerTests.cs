using Bogus;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Caching;
using Logitar.Kraken.Core.Localization;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Tokens;
using Logitar.Kraken.Core.Users;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Kraken.Core.Configurations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class InitializeConfigurationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<ICacheService> _cacheService = new();
  private readonly Mock<IConfigurationQuerier> _configurationQuerier = new();
  private readonly Mock<IConfigurationRepository> _configurationRepository = new();
  private readonly Mock<ILanguageManager> _languageManager = new();
  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly Mock<ISecretHelper> _secretHelper = new();
  private readonly Mock<IUserManager> _userManager = new();

  private readonly InitializeConfigurationCommandHandler _handler;

  public InitializeConfigurationCommandHandlerTests()
  {
    _handler = new(_cacheService.Object, _configurationQuerier.Object, _configurationRepository.Object, _languageManager.Object, _passwordManager.Object, _secretHelper.Object, _userManager.Object);
  }

  [Fact(DisplayName = "It should initialize the configuration when it is not.")]
  public async Task Given_NotInitialized_When_Handle_Then_Initialized()
  {
    InitializeConfigurationCommand command = new(_faker.Locale, _faker.Person.UserName, _faker.Internet.Password());

    Secret secret = new(RandomStringGenerator.GetString());
    _secretHelper.Setup(x => x.Generate(null)).Returns(secret);

    Base64Password password = new(command.Password);
    _passwordManager.Setup(x => x.ValidateAndHash(It.IsAny<IPasswordSettings>(), command.Password)).Returns(password);

    await _handler.Handle(command, _cancellationToken);

    _passwordManager.Verify(x => x.ValidateAndHash(It.IsAny<IPasswordSettings>(), command.Password), Times.Once());
    _configurationRepository.Verify(x => x.SaveAsync(It.Is<Configuration>(y => y.Secret.Equals(secret)), _cancellationToken), Times.Once());
    _languageManager.Verify(x => x.SaveAsync(It.Is<Language>(y => y.IsDefault && y.Locale.Code == command.DefaultLocale), _cancellationToken), Times.Once());
    _userManager.Verify(x => x.SaveAsync(It.Is<User>(y => y.UniqueName.Value == command.UniqueName && y.HasPassword), _cancellationToken), Times.Once());
  }

  [Fact(DisplayName = "It should only cache the configuration when it is already initialized.")]
  public async Task Given_Initialized_When_Handle_Then_OnlyCached()
  {
    Secret secret = new(RandomStringGenerator.GetString());
    Configuration configuration = Configuration.Initialize(secret, ActorId.NewId());
    _configurationRepository.Setup(x => x.LoadAsync(_cancellationToken)).ReturnsAsync(configuration);

    ConfigurationModel model = new();
    _configurationQuerier.Setup(x => x.ReadAsync(configuration, _cancellationToken)).ReturnsAsync(model);

    InitializeConfigurationCommand command = new(_faker.Locale, _faker.Person.UserName, _faker.Internet.Password());
    await _handler.Handle(command, _cancellationToken);

    _cacheService.VerifySet(x => x.Configuration = model);

    _configurationRepository.Verify(x => x.LoadAsync(_cancellationToken), Times.Once());
    _configurationRepository.VerifyNoOtherCalls();

    _languageManager.VerifyNoOtherCalls();
    _passwordManager.VerifyNoOtherCalls();
    _secretHelper.VerifyNoOtherCalls();
    _userManager.VerifyNoOtherCalls();
  }
}
